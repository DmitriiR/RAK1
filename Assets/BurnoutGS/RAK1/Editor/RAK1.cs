//   ___    _   _  __  _   __  
//  | _ \  /_\ | |/ / / | /  \ 
//  |   / / _ \| ' <  | || () |
//  |_|_\/_/ \_\_|\_\ |_(_)__/ 
//***************************************************************//
//	File:		RAK1.cs
//	Author:		Dmitrii Roets
//	Purpose:	Inspector widget and model manager tool 
//**************************************************************//

using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Resources;

// TODO 1/10/17
// Add the + - buttons next to the model count
// Rmove the skip model end list extra code
// write injester for .fbx formats on model lading
// single file import
// fix load error for textures


// select multiple files during 
// add features to the howto file
// add more space for the logo
// remove the debug button (for the build)
// add remove all models button 
// model by model. 
// fix the OffTheWall in tab replace with GS
// add button for normal objects, add create shared / unique for the custom buttons
// change skin to texture
// add the "Add textures" to the decals, add nudge buttons UP,DOWN,<,> 
//&#x2190 code for keys use 



//[CustomEditor(typeof(RAK1_0)), CanEditMultipleObjects]
public class OffTheWall : EditorWindow
{
    // class defines
    private bool _DEBUG = false;
    
    // procedural materials params
    public const string PROC_TILE       = "BrickTile";
    public const string PROC_HUE        = "Brick_Hue";
    public const string PROC_SATURATION = "Brick_Saturation";
    public const string PROC_LIGHTNESS  = "Brick_Lighness";
    public const string PROC_GROUT_HUE  = "Grout_Hue";
    public const string PROC_GROUT_SAT  = "Grout_Saturation";
    public const string PROC_GROUT_LIGHTNESS = "Grout_Lightness";
    public const uint WINDOWN_WIDTH     = 200;
    public const uint WINDOW_HEIGHT     = 200;

    // paths
    static string  materialsPath =      "Assets/BurnoutGS/RAK1/Materials/Props/";
    static string  userPath =           "Assets/BurnoutGS/RAK1/UserModels/";
    static string  hourGlassPath =      "Assets/BurnoutGS/RAK1/Prefabs/Props/Hourglass/";
    static string  modelsPath =         "Assets/BurnoutGS/RAK1/Prefabs/Props/";
    static string  decalsPath =         "Assets/BurnoutGS/RAK1/Prefabs/Decals/";
    static string  curevedSignPath =    "Assets/BurnoutGS/RAK1/Textures/CurvedSignAlbedo/";
    static string  dimondSignPath  =    "Assets/BurnoutGS/RAK1/Textures/DiamondSignAlbedo/";
    static string  sharpSignPath =      "Assets/BurnoutGS/RAK1/Textures/SharpSignAlbedo/";
    static string  triangleSignPath =   "Assets/BurnoutGS/RAK1/Textures/TrianglSignAlbedo/";
    static string  frameSignPath =      "Assets/BurnoutGS/RAK1/Textures/FramePictures/";
    static string  logoPath =           "Assets/BurnoutGS/RAK1/Logo/";

    // private
   
    private static int curvedskin_number = 0;
    private static int dimondskin_number = 0;
    private static int sharpskin_number = 0;
    private static int triangleskin_number = 0;
    private static int userskin_number = 0;
    private static int usermodel_number = 0;
    private static int frameskin_number = 0;
    private static int model_number = 0;
    private static int user_number = 0;
    private static int decal_number = 0;
    private static int tab = 0;
    private static bool repaint_editor = true;
    private static bool reachedEndofList = false;
    //containers 
    private static List<Material>   mL_materials = new List<Material>();
    private static List<GameObject> mL_meshes = new List<GameObject>();
    public  static List<GameObject> mL_userModels = new List<GameObject>();
    public  static List<Texture2D>  mL_userTextures = new List<Texture2D>();
    private static List<String>     mL_filenames = new List<String>();
    private static List<GameObject> mL_decals = new List<GameObject>();
    private static List<Texture2D>  mL_curvedSign = new List<Texture2D>();
    private static List<Texture2D>  mL_dimondSign = new List<Texture2D>();
    private static List<Texture2D>  mL_sharpSign = new List<Texture2D>();
    private static List<Texture2D>  mL_triangleSign = new List<Texture2D>();
    private static List<Texture2D>  mL_frameSign = new List<Texture2D>();


    // Objects 
    Editor gameObjectEditor;
    private GameObject gameObject;
    Texture2D logo;
    GameObject cube;
    public static EditorWindow window;

    void OnInspectorUpdate()
    {
        // this.Repaint();
    }
    public void OnEnable()
    {
        //  LoadHourHalss();

        if (mL_decals.Count == 0)
        {
            LoadTextures();
        }
        if (mL_meshes.Count == 0)
        {
            LoadModels();
        }
        logo = AssetDatabase.LoadAssetAtPath(logoPath + "logo.png", typeof(Texture2D)) as Texture2D;
        cube = AssetDatabase.LoadAssetAtPath(logoPath + "BlankObj.prefab", typeof(GameObject)) as GameObject;
    }

    // draws the model in the editor window
    public void DirtyObject()
    {
        switch (tab)
        {
            case 0:
                {
                    if (mL_meshes.Count > 0 && !reachedEndofList)
                    {
                        gameObject = mL_meshes[model_number];
                    }
                    break;
                }
            case 1:
                {
                    if (mL_decals.Count > 0)
                    {
                        gameObject = mL_decals[decal_number];
                    }
                    break;
                }
           /*
            case 2:
                {
                    if (mL_userModels.Count > 0)
                    {
                        gameObject = mL_userModels[user_number];
                    }
                    else
                        gameObject = null;
                    break;
                }
            default:
                break;
                */
        }

        //********************************************************************
        // Draw Call 
        //********************************************************************
        if (!gameObjectEditor || (gameObject != null && repaint_editor == true) )
        {
        
            gameObjectEditor = null; 
            gameObjectEditor = Editor.CreateEditor(gameObject);
            repaint_editor = false;
        }
        if (gameObjectEditor && gameObject != null)
        {
           
            repaint_editor = false;
            gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(WINDOWN_WIDTH, WINDOWN_WIDTH), EditorStyles.foldout);
        }
        else
        {
            
        }
    }
    
    // loaders
    // Loads the texture pack from resource folder
    public void Populate()
    {
    }

    void DisplayAddModelPrompt()
    {
        gameObject = cube;
        gameObject.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        cube.GetComponent<Renderer>().sharedMaterial.mainTexture = logo;
        AddUserModelButton();
    }



    // draws the buttons below the preview window
    void ModelSelectionbuttons()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("< Previous"))
        {
            switch (tab)
            {
                case 0:
                    model_number--;
                    repaint_editor = true;
                    if (model_number < 0)
                    {
                        model_number = mL_meshes.Count - 1;
                        reachedEndofList = false;
                    }
                    break;
                case 1:
                    decal_number--;
                    repaint_editor = true;
                    if (decal_number < 0) { decal_number = mL_decals.Count - 1; }
                    break;
                    /*
                case 2:
                    user_number--;
                    repaint_editor = true;
                    if (user_number < 0) { user_number = mL_userModels.Count - 1; }
                    break;
                default:
                    break;
                    */
            }
        }
        
        if (GUILayout.Button("Next >"))
        {
            switch (tab)
            {
                case 0:
                    model_number++;
                    repaint_editor = true;
                    if (model_number == mL_meshes.Count) // if we are on the last model
                    {

                        reachedEndofList = true;
                        
                    }

                    if (model_number > mL_meshes.Count)
                    {
                        reachedEndofList = false;
                        model_number = 0;
                    }


                    break;
                case 1:
                    decal_number++;
                    repaint_editor = true;
                    if (decal_number >= mL_decals.Count) { decal_number = 0; }
                    break;
                    /*
                case 2:
                    user_number++;
                    repaint_editor = true;
                    if (user_number >= mL_userModels.Count) { user_number = 0; }
                    break;
                default:
                    break;
                    */
            }

        }
        GUILayout.EndHorizontal();

        CreateButtons();

        GUILayout.BeginHorizontal();
        
        GUILayout.EndHorizontal();



       
    }

    

    //********************************************************************************//
    // Inspector
    //********************************************************************************//
    public void OnGUI()
    {
       
        int tab_save = tab; // needed for return
       // tab = GUILayout.Toolbar(tab, new string[] { "Models", "Decals", "User" });
        tab = GUILayout.Toolbar(tab, new string[] { "Models", "Decals"});
        if (tab_save != tab)
            repaint_editor = true;

        //****************************************************\
        // Draw Call
        DirtyObject();
        //****************************************************\
        // Model
        EditorGUILayout.BeginHorizontal();
        string modelCounts = "empty";
       
        if (tab == 0) modelCounts = (model_number + 1).ToString() + " of " + mL_meshes.Count.ToString();
        if (tab == 1) modelCounts = (decal_number + 1).ToString() + " of " + mL_decals.Count.ToString();
        if (tab == 2) modelCounts = (user_number + 1).ToString() +  " of " + mL_userModels.Count.ToString();
        EditorGUILayout.LabelField("Model", modelCounts);
        if (GUILayout.Button("+"))
        {
            AddUserModelButton();
        }
        if (GUILayout.Button("-"))
        { }


        EditorGUILayout.EndHorizontal();
 
        ModelSelectionbuttons();        
        /// Tabs 
        switch (tab)
        {
            case 0: // models
                {
                    if (gameObject && !reachedEndofList)
                    {
                        DisplayModels();
                    }
                    else if (reachedEndofList)
                    {
                        DisplayAddModelPrompt();
                    }


                    if (_DEBUG)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Box(model_number.ToString());
                        if (gameObject)
                            GUILayout.Box(gameObject.name.ToString());
                        GUILayout.Box(mL_frameSign.Count.ToString());
                        GUILayout.EndHorizontal();
                    }
                }
                break;
            case 1: // decals
                {

                    EditorGUILayout.LabelField("Skin"); //TextArea("Skin");

                    GUILayout.BeginHorizontal();
                    if (gameObject)
                        DisplayDecals();
                    GUILayout.EndHorizontal();
                    if (_DEBUG)
                    {   GUILayout.BeginHorizontal();
                        GUILayout.Box(decal_number.ToString());
                        if (gameObject)
                            GUILayout.Box(gameObject.name.ToString());
                        GUILayout.Box(mL_curvedSign.Count.ToString());
                        GUILayout.Box(mL_dimondSign.Count.ToString());
                        GUILayout.Box(mL_sharpSign.Count.ToString());
                        GUILayout.Box(mL_triangleSign.Count.ToString());
                        GUILayout.EndHorizontal();
                    }
                }
                break;
            case 2: // user
                {

                
                    DisplayUser();
                
                    if (_DEBUG)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Box(user_number.ToString());
                        if(gameObject)
                        GUILayout.Box(gameObject.name.ToString());
                        GUILayout.EndHorizontal();
                    }
                }
                break;
            default:
                break;
        }

        //***************************************************************\
        // bottom row
        GUILayout.BeginHorizontal();

        if (_DEBUG)
        {
            if (GUILayout.Button("Reload Assets"))
            {
            Populate();
            // LoadModels();
            // LoadDecalsFromFolders();
            }
        }
       
   

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        // ********** Add Buttons Old 
        if (tab == 2)
        {
          
        }




        GUILayout.EndHorizontal(); // add user model

        // logo
        DrawLogo(logo);

        // Debug Toggle
        _DEBUG = GUILayout.Toggle(_DEBUG, "Debug");

      

    }

    struct MaterialInfo
    {
        public Material material;
        public string assetPath;
    };

  

    private static void CreateUniqueMaterial(ref GameObject instance)
    {
        Renderer renderer = instance.GetComponent<Renderer>();

        List<string> matPathList = new List<string>();
        List<MaterialInfo> matInfoList = new List<MaterialInfo>();
        List<ProceduralMaterial> pmList = new List<ProceduralMaterial>();

        Material[] sharedMtls = renderer.sharedMaterials;

        string userMaterialsPath = "Assets/BurnoutGS/RAK1/UserMaterials";
        if (!AssetDatabase.IsValidFolder(userMaterialsPath))
        {
            AssetDatabase.CreateFolder("Assets/BurnoutGS/RAK1", "UserMaterials");
        }

        for (int i = 0; i < renderer.sharedMaterials.Length; i++)
        {
            string matPath = AssetDatabase.GetAssetPath(renderer.sharedMaterials[i]);
            int matIdx = matPathList.FindIndex(x => x == matPath);
            if (matIdx == -1)
            {
                matPathList.Add(matPath);
                string newAssetName = Path.GetFileNameWithoutExtension(matPath);
                string newAssetExt = Path.GetExtension(matPath);

                int copyIdx = 1;
                string newAsset;
                bool fileExists;

                do
                {
                    newAsset = userMaterialsPath + "/" + newAssetName + "_User" + copyIdx + newAssetExt;
                    fileExists = File.Exists(newAsset);
                    copyIdx++;
                } while (fileExists);

                AssetDatabase.CopyAsset(matPath, newAsset);
                if(newAsset.Length != 0)
                AssetDatabase.ImportAsset(newAsset);

                if (renderer.sharedMaterials[i] is ProceduralMaterial)
                {
                    SubstanceImporter pmImporter = (SubstanceImporter)AssetImporter.GetAtPath(newAsset);

                    foreach (ProceduralMaterial pm in pmImporter.GetMaterials())
                    {
                        if (pm.name == renderer.sharedMaterials[i].name)
                        {
                            MaterialInfo matInfo = new MaterialInfo();
                            matInfo.material = pm;
                            matInfo.assetPath = newAsset;

                            matInfoList.Add(matInfo);
                            sharedMtls[i] = pm;

                            if (!pmList.Contains(pm))
                                pmList.Add(pm);

                            break;
                        }
                    }
                }
                else
                {
                    Material newMat = (Material)AssetDatabase.LoadAssetAtPath(newAsset, typeof(Material));

                    MaterialInfo matInfo = new MaterialInfo();
                    matInfo.material = newMat;
                    matInfo.assetPath = newAsset;

                    matInfoList.Add(matInfo);
                    sharedMtls[i] = newMat;
                }
            }
            else
            {
                if (renderer.sharedMaterials[i] is ProceduralMaterial)
                {
                    SubstanceImporter pmImporter = (SubstanceImporter)AssetImporter.GetAtPath(matInfoList[matIdx].assetPath);
                    foreach (ProceduralMaterial pm in pmImporter.GetMaterials())
                    {
                        if (pm.name == renderer.sharedMaterials[i].name)
                        {
                            sharedMtls[i] = pm;

                            if (!pmList.Contains(pm))
                                pmList.Add(pm);

                            break;
                        }
                    }
                }
                else
                    sharedMtls[i] = matInfoList[matIdx].material;
            }
        }

        // Replace canvas picture with panel setting
        foreach (ProceduralMaterial pm in pmList)
        {
            if (pm.HasProceduralProperty("proc_pictureInput"))
            {
                pm.SetProceduralTexture("proc_pictureInput", mL_frameSign[frameskin_number]);
                pm.RebuildTextures();
            }
        }

        renderer.sharedMaterials = sharedMtls;
    }
  
    // *********************************
    // TABS  \
    private void DisplayModels()
    {

        gameObject = mL_meshes[model_number];                                           // new additions, fixed the return to the main models display.

        bool usingUserText = false;

        if(gameObject)
            if (gameObject.name.Contains("Frame")
            && mL_frameSign.Count > 0)

        {
            MeshRenderer[] renderers = gameObject.GetComponents<MeshRenderer>();
            foreach (MeshRenderer renderer in renderers)
            {
                Material[] sharedMaterials = renderer.sharedMaterials;

                for (int i = 0; i < sharedMaterials.Length; i++)
                {
                    if (sharedMaterials[i] &&  sharedMaterials[i].GetType() == typeof(ProceduralMaterial))
                    {
                        ProceduralMaterial pm = (ProceduralMaterial)sharedMaterials[i];
                        if (pm.HasProceduralProperty("proc_pictureInput"))
                            {
                            EditorGUILayout.LabelField("Skin", (frameskin_number + 1).ToString() + "/" + mL_frameSign.Count.ToString());
                            GUILayout.BeginHorizontal();
                            if (_DEBUG)
                            {
                                GUILayout.Box(frameskin_number.ToString());
                            }

                            bool contentChanged = false;
                            
                            // traditional        
                            if (GUILayout.Button("< Previous"))
                            {
                                RemoveMissingUserTextures();

                                    if (!usingUserText)
                                    {
                                        frameskin_number--;
                                        frameskin_number = mod(frameskin_number, mL_frameSign.Count);

                                    }
                                    else
                                    {
                                        userskin_number--;
                                        userskin_number = mod(userskin_number, mL_userTextures.Count);
                                    }
                                    contentChanged = true;
                            }
                            if (GUILayout.Button("Next >"))
                            {
                                RemoveMissingUserTextures();
                                    if (!usingUserText)
                                    {
                                        frameskin_number++;
                                        frameskin_number = mod(frameskin_number, mL_frameSign.Count);

                                    } else
                                    {
                                        userskin_number++;
                                        userskin_number = mod(userskin_number, mL_userTextures.Count);

                                    }
                                    contentChanged = true;
                            }

                            if (contentChanged)
                            {
                                pm.SetProceduralTexture("proc_pictureInput", mL_frameSign[frameskin_number]);

                                    // glue traverse 
                                    if (mL_frameSign.Count == frameskin_number &&  mL_userTextures.Count > 0)
                                    {
                                        if (_DEBUG) Debug.Log("Using user textures");
                                        frameskin_number = userskin_number;
                                        usingUserText = true;
                                        pm.SetProceduralTexture("proc_pictureInput", mL_userTextures[frameskin_number]);
                                        
                                    }

                                pm.RebuildTextures();
                            }

                            GUILayout.EndHorizontal();
                        }
                    }
                }
            }

                AddUserTextureButton();

        }
    }

    private void DisplayDecals()
    {
      
         if (gameObject.name.Contains("Curved")
                 && mL_curvedSign.Count > 0)
            {

            Material tempMaterial = new Material(Shader.Find("Standard"));

            if (GUILayout.Button("< Previous"))
            {
                curvedskin_number--;
                repaint_editor = true;
                if (curvedskin_number < 0) { curvedskin_number = mL_curvedSign.Count - 1; }
            }
            if (GUILayout.Button("Next >"))
            {
                repaint_editor = true;
                curvedskin_number++; if (curvedskin_number >= mL_curvedSign.Count) { curvedskin_number = 0; }
            }

            tempMaterial.SetTexture("_MainTex", mL_curvedSign[curvedskin_number]);
            gameObject.GetComponent<MeshRenderer>().material = tempMaterial;
        }
        if (gameObject.name.Contains("Diamond") && mL_dimondSign.Count > 0)
        {
            Material tempMaterial = new Material(Shader.Find("Standard"));

            if (GUILayout.Button("< Previous"))
            {
                dimondskin_number--;
                repaint_editor = true;
                if (dimondskin_number < 0) { dimondskin_number = mL_dimondSign.Count - 1; }
            }
            if (GUILayout.Button("Next >"))
            {
                dimondskin_number++;
                repaint_editor = true;
                if (dimondskin_number >= mL_dimondSign.Count) { dimondskin_number = 0; }
            }

            tempMaterial.SetTexture("_MainTex", mL_dimondSign[dimondskin_number]);
            gameObject.GetComponent<MeshRenderer>().material = tempMaterial;
        }
      
        if (gameObject.name.Contains("Sharp") && mL_sharpSign.Count > 0)
        {

            Material tempMaterial = new Material(Shader.Find("Standard"));

            if (GUILayout.Button("< Previous"))
            {
                sharpskin_number--;
                repaint_editor = true;
                if (sharpskin_number < 0) { sharpskin_number = mL_sharpSign.Count - 1; }
            }
            if (GUILayout.Button("Next >"))
            {
                sharpskin_number++;
                repaint_editor = true;
                if (sharpskin_number >= mL_sharpSign.Count) { sharpskin_number = 0; }
            }

            tempMaterial.SetTexture("_MainTex", mL_sharpSign[sharpskin_number]);
            gameObject.GetComponent<MeshRenderer>().material = tempMaterial;
        }
     
        if  (gameObject.name.Contains("Triangl") && mL_triangleSign.Count > 0)
        {
            Material tempMaterial = new Material(Shader.Find("Standard"));

            if (GUILayout.Button("< Previous"))
            {
                triangleskin_number--;
                repaint_editor = true;
                if (triangleskin_number < 0) { triangleskin_number = mL_triangleSign.Count - 1; }
            }
            if (GUILayout.Button("Next >"))
            {
                triangleskin_number++;
                repaint_editor = true;
                if (triangleskin_number >= mL_triangleSign.Count) { triangleskin_number = 0; }
            }

            tempMaterial.SetTexture("_MainTex", mL_triangleSign[triangleskin_number]);
            gameObject.GetComponent<MeshRenderer>().material = tempMaterial;
        }

    }

    private void DisplayUser()
    {
        if (mL_userModels.Count > 0)
        {
            EditorGUILayout.LabelField("Models"); //TextArea("Skin");
            if (GUILayout.Button("< Previous"))
            {
                usermodel_number--;
                if (usermodel_number < 0) { usermodel_number = mL_userModels.Count - 1; }
                repaint_editor = true;
            }
            if (GUILayout.Button("Next >"))
            {
                usermodel_number++; if (usermodel_number >= mL_userModels.Count) { usermodel_number = 0; }
                repaint_editor = true;
            }
        }

        if (mL_userTextures.Count > 0)
        {
            EditorGUILayout.LabelField("Textures"); //TextArea("Skin");
            if (GUILayout.Button("< Previous"))
            {
                userskin_number--;
                if (userskin_number < 0) { userskin_number = mL_userTextures.Count - 1; }
                repaint_editor = true;
            }
            if (GUILayout.Button("Next >"))
            {
                userskin_number++;
                if (userskin_number >= mL_userTextures.Count) { userskin_number = 0; }
                repaint_editor = true;
            }
        }
    }
    // end tabs
    // *********************************
    void CreateButtons()
    {
        if (GUILayout.Button("Create Shared") && gameObject)
        {
            float size = 1.0f;
            Renderer renderer = gameObject.GetComponent<Renderer>();

            // Put object in front of camera
            if (renderer)
            {
                size = Mathf.Max(size, renderer.bounds.size.x, renderer.bounds.size.y, renderer.bounds.size.z);
            }

            Camera viewCamera = SceneView.lastActiveSceneView.camera;
            Vector3 objPos = viewCamera.transform.position + viewCamera.transform.forward * size;

            // Move object onto surface that being looked at
            Ray ray = viewCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 10.0f))
            {
                objPos = hitInfo.point;
            }

            GameObject instance = (GameObject)Instantiate(gameObject, objPos, Quaternion.identity);


            //   if (mb_createNewMaterial && tab == 0)
            //   {
            //        CreateUniqueMaterial(ref instance);
            // add to the user container                                                         <<<<< * New feature 
            // mL_userModels.Add(instance);

            //    }
            //   // Remove 'Clone' from object name
            instance.name = gameObject.name;

        }
        if (GUILayout.Button("Create Unique") && gameObject)
        {
            float size = 1.0f;
            Renderer renderer = gameObject.GetComponent<Renderer>();

            // Put object in front of camera
            if (renderer)
            {
                size = Mathf.Max(size, renderer.bounds.size.x, renderer.bounds.size.y, renderer.bounds.size.z);
            }

            Camera viewCamera = SceneView.lastActiveSceneView.camera;
            Vector3 objPos = viewCamera.transform.position + viewCamera.transform.forward * size;

            // Move object onto surface that being looked at
            Ray ray = viewCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 10.0f))
            {
                objPos = hitInfo.point;
            }

            GameObject instance = (GameObject)Instantiate(gameObject, objPos, Quaternion.identity);


            CreateUniqueMaterial(ref instance);
            //  if (mb_createNewMaterial && tab == 0)
            // if (tab == 0)
            //   {
            // add to the user container                                                         <<<<< * New feature 
            // mL_userModels.Add(instance);
            //   }
            // Remove 'Clone' from object name
            instance.name = gameObject.name;

        }

    }

    public static void LoadModels()
    {
        //*****************************************************************//
        // Models  
        LoadGOListAtPath(modelsPath, mL_meshes, ".prefab");
        //*****************************************************************//
        // Decals
        LoadGOListAtPath(decalsPath, mL_decals, ".prefab");
        //*****************************************************************//
        // User
        LoadGOListAtPath(userPath, mL_userModels, ".prefab");
    }

    public static void LoadTextures()
    {
        //*****************************************************************//
        // Substances 
        LoadMATListAtPath(materialsPath, mL_materials, ".sbsar");
        //*****************************************************************//
        // textures 
        LoadTEX2DListAtPath(curevedSignPath, mL_curvedSign, ".png");
        LoadTEX2DListAtPath(dimondSignPath, mL_dimondSign, ".png");
        LoadTEX2DListAtPath(sharpSignPath, mL_sharpSign, ".png");
        LoadTEX2DListAtPath(triangleSignPath, mL_triangleSign, ".png");
        LoadTEX2DListAtPath(frameSignPath, mL_frameSign, ".png");
        LoadTEX2DListAtPath(frameSignPath, mL_frameSign, ".tga");
    }

    public static void LoadGOListAtPath(string _path, List<GameObject> _list, string _ext)
    {
        string assetpath = _path;
        DirectoryInfo dir = new DirectoryInfo(assetpath);
        FileInfo[] info = dir.GetFiles("*.*");
        float progress = 1.0f;
        foreach (FileInfo file in info)
        {
            if (file.Extension == _ext)
            {
                string temp_Name = file.Name;
                string extension = file.Extension;
                string strippedName = temp_Name.Replace(extension, "");
                mL_filenames.Add(strippedName);
                GameObject temp_mesh = AssetDatabase.LoadAssetAtPath(assetpath + temp_Name, typeof(GameObject)) as GameObject;
                if (temp_mesh)
                    _list.Add(temp_mesh);
                // load progress bar.
                EditorUtility.DisplayProgressBar("Loading Assets", strippedName, progress / info.Length);
            }
        }
        EditorUtility.ClearProgressBar();
    }
    // loads the material array 
    public static void LoadMATListAtPath(string _path, List<Material> _list, string _ext)
    {
        string assetpath = _path;
        DirectoryInfo dir = new DirectoryInfo(assetpath);
        FileInfo[] info = dir.GetFiles("*.*");
        foreach (FileInfo file in info)
        {
            if (file.Extension == _ext)
            {
                string temp_Name = file.Name;
                string extension = file.Extension;
                string strippedName = temp_Name.Replace(extension, "");
                mL_filenames.Add(strippedName);
                Material temp_material = AssetDatabase.LoadAssetAtPath(assetpath + temp_Name, typeof(Material)) as Material;
                if (temp_material)
                    _list.Add(temp_material);

            }
        }
    }
    // loads the texture 2D structure 
    public static void LoadTEX2DListAtPath(string _path, List<Texture2D> _list, string _ext)
    {
        string assetpath = _path;
        DirectoryInfo dir = new DirectoryInfo(assetpath);
        FileInfo[] info = dir.GetFiles("*.*");
        foreach (FileInfo file in info)
        {
            if (file.Extension == _ext)
            {
                string temp_Name = file.Name;
                string extension = file.Extension;
                string strippedName = temp_Name.Replace(extension, "");
                mL_filenames.Add(strippedName);
                Texture2D temp_material = AssetDatabase.LoadAssetAtPath(assetpath + temp_Name, typeof(Texture2D)) as Texture2D;

                if (temp_material)
                    _list.Add(temp_material);
            }
        }
    }

    public void LoadHourHalss()
    {
        LoadGOListAtPath(hourGlassPath, mL_userModels, ".fbx");
    }
    // end loaders 

    void AddUserModelButton()
    {
          string path = EditorUtility.OpenFilePanel("Add Models", "", "fbx,prefab");

            if (path.Length != 0)
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] info = dir.GetFiles("*.*");

                // Create UserTextures folder if not exist
                string userModelsPath = "Assets/BurnoutGS/RAK1/UserModels";
                if (!AssetDatabase.IsValidFolder(userModelsPath))
                {
                    AssetDatabase.CreateFolder("Assets/BurnoutGS/RAK1", "UserModels");
                }

                float progress = 1.0f;
                foreach (FileInfo file in info)
                {
                    string extent = file.Extension;
                    if (extent == ".fbx" || extent == ".prefab")
                    {
                        string filename = file.Name;
                        string extension = file.Extension;
                        string strippedName = filename.Replace(extension, "");
                        mL_filenames.Add(strippedName);
                        string assetPath = "Assets/BurnoutGS/RAK1/UserModels/" + filename;

                        EditorUtility.DisplayProgressBar("Importing user Models", "Importing " + assetPath, progress / info.Length);

                        // Copy to UserModels folder
                        try
                        {
                            File.Copy(file.FullName, Application.dataPath + "/BurnoutGS/RAK1/UserModels/" + filename);
                        }
                        catch
                        {
                            Debug.Log("could not copy " + filename.ToString());
                        }
                        AssetDatabase.ImportAsset(assetPath);
                        GameObject temp_object = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                        UnityEngine.Object prefab = PrefabUtility.GetPrefabObject(temp_object);

                        if (temp_object)
                            mL_meshes.Add((GameObject)prefab);
                    }

                    progress += 1.0f;
                }

                EditorUtility.ClearProgressBar();

            }
       // }

    }
    void AddUserTextureButton()
    {
        if (GUILayout.Button("Add User Textures"))
        {
            string path = EditorUtility.OpenFolderPanel("Load Textures at Directory", "", "");

            if (path.Length != 0)
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] info = dir.GetFiles("*.*");

                // Create UserTextures folder if not exist
                string userModelsPath = "Assets/BurnoutGS/RAK1/UserTextures";
                if (!AssetDatabase.IsValidFolder(userModelsPath))
                {
                    AssetDatabase.CreateFolder("Assets/BurnoutGS/RAK1", "UserTextures");
                }

                float progress = 1.0f;
                foreach (FileInfo file in info)
                {
                    string extent = file.Extension;

                    if (IsTexture2D(extent))
                    {
                        string filename = file.Name;
                        string extension = file.Extension;
                        string strippedName = filename.Replace(extension, "");
                        mL_filenames.Add(strippedName);
                        string assetPath = "Assets/BurnoutGS/RAK1/UserTGextures/" + filename;

                        EditorUtility.DisplayProgressBar("Importing user Textures", "Importing " + assetPath, progress / info.Length);

                        // Copy to UserModels folder
                        try
                        {
                            File.Copy(file.FullName, Application.dataPath + "/BurnoutGS/RAK1/UserTextures/" + filename);
                        }
                        catch
                        {
                            if (_DEBUG) Debug.Log("could not copy " + filename.ToString());
                        }
                        AssetDatabase.ImportAsset(assetPath);
                        GameObject temp_object = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                        UnityEngine.Object prefab = PrefabUtility.GetPrefabObject(temp_object);


                        if (temp_object)
                            mL_userTextures.Add((Texture2D)prefab);
                    }

                    progress += 1.0f;
                }

                EditorUtility.ClearProgressBar();

            }
        }
    }


    void RemoveMissingUserTextures()
    {
        for (int i = 0; i < mL_frameSign.Count;)
        {
            if (!mL_frameSign[i])
                mL_frameSign.RemoveAt(i);
            else
                i++;
        }
    }
    private static void DrawLogo(Texture2D _logo)
    {

        //////////////////////////////////// TODO

        // move the logo to far right


        if (window)
        {
            float height = window.position.height;
            float width = window.position.width;
            float logoShrinkFactor = 16.0f;     // the higher, the smaller the logo
            if (_logo)
            {

                if (height < 400.0f)
                    GUI.DrawTexture(new Rect(width - _logo.width / logoShrinkFactor,                                         //x
                                        400 - _logo.height / logoShrinkFactor,  //y
                                           _logo.width / logoShrinkFactor,         //width
                                           _logo.height / logoShrinkFactor),       // height
                                           _logo);
                else
                    GUI.DrawTexture(new Rect(width - _logo.width / logoShrinkFactor,                                         //x
                                            height - _logo.height / logoShrinkFactor, //y
                                             _logo.width / logoShrinkFactor,           //width
                                             _logo.height / logoShrinkFactor),         // height
                                             _logo);
            }
        }
        else
        {
            GUILayout.Box("window is null");
            float logoShrinkFactor = 16.0f;
            GUI.DrawTexture(new Rect(0, 420 - _logo.height / logoShrinkFactor, _logo.width / logoShrinkFactor, _logo.height / logoShrinkFactor), _logo);
        }

    }
    // toolbar stuff

    [MenuItem("BurnoutGS/Show Panel")]
    static void ShowPanel()
    {
        
        EditorUtility.DisplayProgressBar("Loading", "", 0.5f);
        OffTheWall.window = EditorWindow.GetWindow(typeof(OffTheWall));
        EditorUtility.ClearProgressBar();
        //  window.Close();
    }

    [MenuItem("BurnoutGS/Close")]
    static void ClosePanel()
    {
        if(window)
           window.Close();
    }

    [MenuItem("BurnoutGS/Refresh")]
    static void Refresh()
    {
        mL_meshes.Clear();
        mL_curvedSign.Clear();
        mL_decals.Clear();
        mL_dimondSign.Clear();
        mL_filenames.Clear();
        mL_frameSign.Clear();
        mL_materials.Clear();
        mL_meshes.Clear();
        mL_sharpSign.Clear();
        mL_triangleSign.Clear();
        mL_userModels.Clear();
        mL_userTextures.Clear();
        LoadTextures();
        OffTheWall.LoadModels();       
        
    }


    // helpers
    // TODO : make seperate file for these  

    int mod(int x, int m)
    {
        return (x % m + m) % m;
    }
    private bool IsTexture2D(string ext)
    {
        return (ext == ".psd" ||
                ext == ".tiff" ||
                ext == ".jpg" ||
                ext == ".tga" ||
                ext == ".png" ||
                ext == ".gif" ||
                ext == ".bmp" ||
                ext == ".iff" ||
                ext == ".pict");
    }
}



