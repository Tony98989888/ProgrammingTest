using System.IO;

public static class DialogEditorDirConst
{
    public static readonly string RootPath = "Assets";
    public static readonly string DialogSaveFolderName = "DialogSaveFile";
    public static readonly string DialogSaveSubfolderName = "Dialogues";

    public static string GetGraphSavePath(string graphName) 
    {
        return Path.Combine(RootPath, DialogSaveFolderName, DialogSaveSubfolderName, graphName);
    }



}
