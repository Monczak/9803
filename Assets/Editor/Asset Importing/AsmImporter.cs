using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace NineEightOhThree.Utilities.AssetImporting
{
    [ScriptedImporter( 1, "asm" )]
    public class AsmImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            TextAsset asset = new TextAsset(File.ReadAllText(ctx.assetPath));
            ctx.AddObjectToAsset("text", asset);
            ctx.SetMainObject(asset);
        }
    }
}