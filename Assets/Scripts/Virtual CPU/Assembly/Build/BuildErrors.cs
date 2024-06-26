﻿namespace NineEightOhThree.VirtualCPU.Assembly.Build
{
    public static class BuildErrors
    {
        public static BuildError ResourceNotFound(BuildJob job) =>
            new(job, $"Resource {job.ResourceLocation} not found");
        
        public static BuildError AssemblerFailed(BuildJob job) =>
            new(job, $"Building {job.ResourceLocation} failed due to assembler errors", job.Result.Errors);
        
        public static BuildError OverlappingCode(BuildJob job1, BuildJob job2, ushort address) =>
            new(job1, $"Overlapping code from {job1.ResourceLocation} and {job2.ResourceLocation} at address 0x{address:X4}");
    }
}