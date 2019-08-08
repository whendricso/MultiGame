#pragma warning disable 0618
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MultiGame;

namespace MultiGame{
    [System.Serializable]
    public struct TerrainObject
    {
        public float[, ,] splatmapData;
        public float[,] heightmapData;

        public int alphamapWidth;
        public int alphamapHeight;
        public int alphamapLayers;
        public int alphaMapResolution;
        //colors
        public List<float> alphamapTextureColor_r; 
        public List<float> alphamapTextureColor_g; 
        public List<float> alphamapTextureColor_b; 
        public List<float> alphamapTextureColor_a; 

        public string splatmapName;
        public int splatmapProtoTextureMIP;
        public float splatmapProtoTextureTileSizex;
        public float splatmapProtoTextureTileSizey;
        public float splatmapProtoTextureTileOffsetx; 
        public float splatmapProtoTextureTileOffsety; 
        public float splatmapProtoTextureSmoothness;

        public List<float> splatmapProtoTextureColor_r; 
        public List<float> splatmapProtoTextureColor_b; 
        public List<float> splatmapProtoTextureColor_g; 
        public List<float> splatmapProtoTextureColor_a; 
        public int baseMapResolution;

        //transforms
        public float positionx;
        public float positiony;
        public float positionz;
        public float rotationx;
        public float rotationy;
        public float rotationz;
        public float rotationw;
        public float scalex;
        public float scaley;
        public float scalez;

        // details
        public int detailHeight;
        public DetailPrototype[] detailPrototypes;
        //public float detailPrototypes;
        // IF terraindata has zero detail resolution, then don't worry about GetDetailLayers()
        public int detailResolution;
        public int detailWidth;
        public int heightmapHeight;
        public int heightmapResolution;
        public float heightmapScalex;
        public float heightmapScaley;
        public float heightmapScalez;
        public float sizex;
        public float sizey;
        public float sizez;

        private int GetIndexX(int i){
            return i % 512;
        }
        private int GetIndexY(int i){
            return (int)Mathf.Floor(i / 512);
        }
        private void WriteTextureToPNG(Texture2D texture, string name)
        {
            byte[] bytes = texture.EncodeToPNG();

            string tempPath = Application.persistentDataPath;
            File.WriteAllBytes(tempPath + "/" + name + ".png", bytes);
        }
        private Color[] buildColors(){
            Color[] result = new Color[splatmapProtoTextureColor_r.Count];
            for(int i = 0; i < splatmapProtoTextureColor_r.Count; i++){
                result[i] = new Color(splatmapProtoTextureColor_r[i], splatmapProtoTextureColor_g[i],
                                        splatmapProtoTextureColor_b[i], splatmapProtoTextureColor_a[i]);
                // if(i<1000){
                //     Debug.Log("Out Colors index: " + i + " -- " + result[i].ToString());
                //     Debug.Log("Pixel at i(" + i + "): " + realTexture.GetPixel(GetIndexX(i),GetIndexY(i)).ToString());
                // }
            }
            return result;
        }
        public SplatPrototype[] GetSplatPrototypes(){
            // Color[] colors = buildColors();
            SplatPrototype[] result = new SplatPrototype[1];
            SplatPrototype splatProto = new SplatPrototype();
            // splatProto.tileSize = new Vector2(splatmapProtoTextureTileSizex, splatmapProtoTextureTileSizey);
            // splatProto.tileOffset = new Vector2(splatmapProtoTextureTileOffsetx, splatmapProtoTextureTileOffsety);
            // splatProto.smoothness = splatmapProtoTextureSmoothness;
            // Texture2D texture = new Texture2D(alphamapWidth, alphamapHeight);
            // for(int i = 0; i < 512; i++){
            //     for(int j = 0; j < 512; j++){
            //         texture.SetPixel(i,j, colors[i*512 + j]);
            //     }
            // }
            // texture.Apply();
            //texture.SetPixels(colors);

            // Debug.Log ( "tilesize: " + result[0].tileSize.ToString());
            // for(int i = 0; i < 1000; i++){
            //     if(i%100 == 0)
            //         Debug.Log ( "normals: " + result[0].normalMap.GetPixel(i, 10).ToString());
                
            // }

        
            Texture2D mtexture = Resources.Load(splatmapName) as Texture2D;
            if(!mtexture)
                Debug.LogError("Failed Load Texture: " + splatmapName);
            splatProto.texture = mtexture;
            // splatProto.texture.Apply();
            // splatProto.texture.name = splatmapName;
            result[0] = splatProto;
            return result;
        }
        public Vector3 GetScale(){
            return new Vector3(scalex, scaley, scalez);
        }
        public Vector3 GetPosition(){
            return new Vector3(positionx, positiony, positionz);
        }
        public Quaternion GetRotation(){
            return new Quaternion(rotationx, rotationy, rotationz, rotationw);
        }
        public Vector3 GetSize(){
            return new Vector3(sizex, sizey, sizez);
        }
        public TerrainObject(GameObject _tobj, TerrainData _tdata, Texture2D[] _text2dArr, int _alphamapWidth, int _alphamapHeight, int _alphamapLayers, int _alphamapResolution, int _baseMapResolution,
                            int _detailHeight, DetailPrototype[] _detailPrototypes, int _detailResolution, int _detailWidth, int _heightMapHeight, int _heightMapResolution,
                            Vector3 _heightMapScale, Vector3 _size) 
        {
            if(_tdata.splatPrototypes.Length > 0){
                Texture2D[] splatTextures = {_tdata.splatPrototypes[0].texture};
                splatmapName = splatTextures[0].name;
                splatmapProtoTextureMIP = splatTextures[0].mipmapCount;
                splatmapProtoTextureTileSizex = _tdata.splatPrototypes[0].tileSize.x;
                splatmapProtoTextureTileSizey = _tdata.splatPrototypes[0].tileSize.y;
                splatmapProtoTextureTileOffsetx = _tdata.splatPrototypes[0].tileOffset.x;
                splatmapProtoTextureTileOffsety = _tdata.splatPrototypes[0].tileOffset.y;
                splatmapProtoTextureSmoothness = _tdata.splatPrototypes[0].smoothness;
                splatmapProtoTextureColor_r = AssignColorsFromTexture(splatTextures, new ProcessColorDelegate(ProcessRed),splatmapProtoTextureMIP );
                splatmapProtoTextureColor_g = AssignColorsFromTexture(splatTextures, new ProcessColorDelegate(ProcessGreen),splatmapProtoTextureMIP);
                splatmapProtoTextureColor_b = AssignColorsFromTexture(splatTextures, new ProcessColorDelegate(ProcessBlue),splatmapProtoTextureMIP);
                splatmapProtoTextureColor_a = AssignColorsFromTexture(splatTextures, new ProcessColorDelegate(ProcessAlpha),splatmapProtoTextureMIP);
            } else {
                Debug.LogError("No SplatPrototypes");
                List<float> zeroList = new List<float>(1);
                zeroList.Add(0);
                splatmapName = "";
                splatmapProtoTextureColor_a = zeroList;
                splatmapProtoTextureColor_r = zeroList;
                splatmapProtoTextureColor_g = zeroList;
                splatmapProtoTextureColor_b = zeroList;
                splatmapProtoTextureTileSizex = 0;
                splatmapProtoTextureTileSizey = 0;
                splatmapProtoTextureTileOffsetx = 0;
                splatmapProtoTextureTileOffsety = 0;
                splatmapProtoTextureSmoothness = 0;
                splatmapProtoTextureMIP = 0;
            }

            alphamapTextureColor_r = AssignColorsFromTexture(_text2dArr, new ProcessColorDelegate(ProcessRed), _text2dArr[0].mipmapCount);
            alphamapTextureColor_g = AssignColorsFromTexture(_text2dArr, new ProcessColorDelegate(ProcessGreen), _text2dArr[0].mipmapCount);
            alphamapTextureColor_b = AssignColorsFromTexture(_text2dArr, new ProcessColorDelegate(ProcessBlue), _text2dArr[0].mipmapCount);
            alphamapTextureColor_a = AssignColorsFromTexture(_text2dArr, new ProcessColorDelegate(ProcessAlpha), _text2dArr[0].mipmapCount);
            // transforms
            positionx = _tobj.transform.position.x;
            positiony = _tobj.transform.position.y;
            positionz = _tobj.transform.position.z;
            rotationx = _tobj.transform.rotation.x;
            rotationy = _tobj.transform.rotation.y;
            rotationz = _tobj.transform.rotation.z;
            rotationw = _tobj.transform.rotation.w;
            scalex = _tobj.transform.localScale.x;
            scaley = _tobj.transform.localScale.y;
            scalez = _tobj.transform.localScale.z;

            // TODO add support for multiple textures
            // for(int i = 0; i < 1000; i++){
            //     if(i%100 == 0)
            //         Debug.Log ( "normals: " + _tdata.splatPrototypes[0].normalMap);
            // }
            splatmapData =        _tdata.GetAlphamaps(0,0, _alphamapWidth, _alphamapHeight);
            //LogSplatData(splatmapData);
            heightmapData =       _tdata.GetHeights(0,0, _alphamapWidth, _alphamapHeight);
            alphamapWidth =       _alphamapWidth;
            alphamapHeight =      _alphamapHeight;
            alphamapLayers =      _alphamapLayers;
            alphaMapResolution =  _alphamapResolution;
            baseMapResolution =   _baseMapResolution;

            detailHeight =        _detailHeight;
            detailPrototypes =    _detailPrototypes;
            detailResolution =    _detailResolution;
            detailWidth =         _detailWidth;
            heightmapHeight =     _heightMapHeight;
            heightmapResolution = _heightMapResolution;
            heightmapScalex =      _heightMapScale.x;
            heightmapScaley =      _heightMapScale.y;
            heightmapScalez =      _heightMapScale.z;
            sizex =                _size.x;
            sizey =                _size.y;
            sizez =                _size.z;
        }
        static float ProcessRed(Color color){
            return color.r;
        }
        static float ProcessGreen(Color color){
            return color.g;
        }
        static float ProcessBlue(Color color){
            return color.b;
        }
        static float ProcessAlpha(Color color){
            return color.a;
        }

        public delegate float ProcessColorDelegate(Color color); //ew
        public static List<float> AssignColorsFromTexture(Texture2D[] textures, ProcessColorDelegate processColor, int miplevel){
            List<float> colorValues = new List<float>();
            if(textures.Length > 1){
                Debug.LogError("Expecting texture length to be 1. It is greater then 1");
            }
            for(int i=0; i < textures.Length; i++){
                //Debug.Log("MIPLEVEL: " + miplevel);
                Color[] colors = textures[i].GetPixels();
                colorValues = new List<float>(colors.Length);
                for(int j=0; j < colors.Length; j++){
                    // if(j%1000 == 1)
                    //     Debug.Log("In Colors index: " + j + " -- " + colors[j].ToString());
                    colorValues.Add( processColor(colors[j]) );
                }
            }
            return colorValues;
        }
    }
}