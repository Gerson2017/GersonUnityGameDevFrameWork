using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


/// <summary>
/// 定制图片导入格式和大小
/// </summary>
public class TextureSetting : AssetPostprocessor//资源后处理
{


    //在图片导入前
    void OnPreprocessTexture()
    {
        TextureImporter importer = (TextureImporter)assetImporter;
        if (assetImporter.assetPath.Contains("Assets/UI"))
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.maxTextureSize= importer.maxTextureSize;

            //设置各平台的压缩格式
            TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings("Android");

             settings.maxTextureSize = settings.maxTextureSize;
  
            settings.overridden = true;
            settings.format = TextureImporterFormat.ASTC_5x5;
            importer.SetPlatformTextureSettings(settings);
        }  
     

    }
}
