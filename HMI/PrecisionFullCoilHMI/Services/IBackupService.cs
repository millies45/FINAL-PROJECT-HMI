namespace PrecisionFullCoilHMI.Services
{
    public interface IBackupService
    {
        void ExportData(string filePath);
        void RestoreData(string filePath);
    }
}
