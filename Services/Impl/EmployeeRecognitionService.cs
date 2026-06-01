using AttendanceManagementApp.Configs;
using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.Exception;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace AttendanceManagementApp.Services.Impl;

public class EmployeeRecognitionService: IEmployeeRecognitionService
{

    private readonly AppDbContext _context;
    private readonly IAttendanceService _attendanceService;
    
    public  EmployeeRecognitionService(AppDbContext context,  IAttendanceService attendanceService)
    {
        _context = context;
        _attendanceService = attendanceService;
    }
    
    public async Task RegisterFaceAsync(EmployeeRecognitionCreateReq req)
    {
        if (string.IsNullOrEmpty(req.Email))
            throw new BadRequestException("Vui lòng cung cấp email");

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Email == req.Email);

        if (employee == null)
            throw new BadRequestException("Nhân viên không tồn tại");

        var entity = new EmployeeRecognition
        {
            EmployeeId = employee.Id,
            FaceEmbedding = req.EmbeddingImage
        };

        _context.EmployeeRecognitions.Add(entity);
        await _context.SaveChangesAsync();
    }
  
    public async Task<string> CheckInByFaceAsync(EmployeeRecognitionCreateReq req)
    {
        if (req.EmbeddingImage == null || req.EmbeddingImage.Length == 0)
            throw new BadRequestException("Embedding không hợp lệ");

        // 1. Lấy toàn bộ embedding từ DB
        var data = await _context.EmployeeRecognitions
            .AsNoTracking()
            .ToListAsync();

        if (!data.Any())
            throw new BadRequestException("Face id không tồn tại trong hệ thống");

        // 2. Tìm embedding gần nhất
        EmployeeRecognition? bestMatch = null;
        double bestScore = -1;

        foreach (var item in data)
        {
            var score = CosineSimilarity(req.EmbeddingImage, item.FaceEmbedding);

            if (score > bestScore)
            {
                bestScore = score;
                bestMatch = item;
            }
        }

        // 3. Threshold (quan trọng)
        const double THRESHOLD = 0.8; // tuỳ model (0.7 - 0.9)

        if (bestMatch == null || bestScore < THRESHOLD)
            throw new BadRequestException("Không thể nhận diện khuôn mặt");

        // 4. Gọi check-in
        var checkInReq = new AttendanceCheckInReq
        {
            EmployeeId = bestMatch.EmployeeId
        }; 
        
        var res = await _attendanceService.CheckInAsync(checkInReq);
        if (res.CheckOut.HasValue)
        {
            return "Check out thành công";
        }

        return "Check in thành công";
    }
    
    private double CosineSimilarity(float[] v1, float[] v2)
    {
        if (v1.Length != v2.Length)
            throw new BadRequestException("Kích thước embedding không khớp.");

        double dot = 0;
        double norm1 = 0;
        double norm2 = 0;

        for (int i = 0; i < v1.Length; i++)
        {
            dot += v1[i] * v2[i];
            norm1 += v1[i] * v1[i];
            norm2 += v2[i] * v2[i];
        }

        return dot / (Math.Sqrt(norm1) * Math.Sqrt(norm2));
    }
}