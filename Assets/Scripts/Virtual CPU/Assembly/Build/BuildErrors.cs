using NineEightOhThree.VirtualCPU.Assembly.Assembler;

namespace NineEightOhThree.VirtualCPU.Assembly.Build
{
    public static class BuildErrors
    {
        public static BuildError JobFailed(BuildJob job) =>
            new(job, $"Building {job.ResourceLocation} failed");
        
        public static BuildError OverlappingCode(BuildJob job1, BuildJob job2, ushort address) =>
            new(job1, $"Overlapping code from {job1.ResourceLocation} and {job2.ResourceLocation} at address 0x{address:X4}");
    }
}