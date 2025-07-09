using System;
using System.Collections.Generic;

namespace ElearnAPI.DTOs
{
    public class CourseProgressDto
    {
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; } = null!;
        public Guid UserId { get; set; }

        public int TotalFiles { get; set; }
        public int CompletedFiles { get; set; }

        public double ProgressPercentage => TotalFiles == 0 ? 0 : Math.Round((double)CompletedFiles / TotalFiles * 100, 2);

        public bool IsCompleted => TotalFiles > 0 && CompletedFiles == TotalFiles;

        public List<FileProgressDetailDto> FileProgresses { get; set; } = new();
    }

    public class FileProgressDetailDto
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; } = null!;
        public string Topic { get; set; } = null!;
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
