using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using UnityEngine;

public class Game : PersistableObject
{
    public ShapeFactory shapeFactory;
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;
    public KeyCode destroyKey = KeyCode.X;
    public float CreationSpeed { get; set; }
    float creationProgress;
    public float DestructionSpeed { get; set; }
    float destructionProgress;

    public PersistentStorage storage;

    public int levelCount;
    int loadedLevelBuildIndex;

    public float radius;
    public float minScale;
    public float maxScale;

    List<Shape> shapes;// = new List<Shape>();
    private string savePath;
    const int saveVersion = 2;

    void Start()
    {
        shapes = new List<Shape>();

        // Checks if correct level is already loaded, if not, load it
        if (Application.isEditor)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.name.Contains("Level "))
                {
                    SceneManager.SetActiveScene(loadedScene);
                    return;
                }
            }
        }
        StartCoroutine(LoadLevel(1));
    }

    // Update is called once per frame
    void Update()
    {
        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f) // Checks if time has counted up, also uses a while statement in case the timer counts up multiple instances so the function can be executed an appropriate amount of times
        {
            creationProgress -= 1f;
            CreateShape();
        }
        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1f) // Checks if time has counted up, also uses a while statement in case the timer counts up multiple instances so the function can be executed an appropriate amount of times
        {
            destructionProgress -= 1f;
            DestroyShape();
        }

        if (Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
        else
        {
            for (int i = 1; i <= levelCount; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    BeginNewGame();
                    StartCoroutine(LoadLevel(i));
                    return;
                }
            }
        }

        if (Input.GetKey(createKey))
        {
            CreateShape();
        }
        else if (Input.GetKey(destroyKey))
        {
            DestroyShape();
        }

        
        if (Input.GetKeyDown(saveKey))
        {
            storage.Save(this, saveVersion);
        }

        if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
        
    }

    IEnumerator LoadLevel(int levelBuildIndex) // Used for loading levels that would take a long time to load. There would ideally be additional code for displaying a loading screen
    {
        enabled = false;
        if (loadedLevelBuildIndex > 0)
        {
            yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        }
        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive); // Loads appropriate scene and pauses execution until new scene is loaded
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex)); // Sets new scene to be active scene
        loadedLevelBuildIndex = levelBuildIndex;
        enabled = true;
    }

    void BeginNewGame()
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            shapeFactory.Reclaim(shapes[i]);
            //Destroy(shapes[i].gameObject);
        }
        shapes.Clear();
    }

    void CreateShape()
    {
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;
        t.localPosition = Random.insideUnitSphere * radius;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(minScale, maxScale);
		instance.SetColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f));
        shapes.Add(instance); // This script runs afterwards so the object is only added once all its important data is set.
    }

    void DestroyShape()
    {
        if (shapes.Count > 0)
        {
            int index = Random.Range(0, shapes.Count);
            shapeFactory.Reclaim(shapes[index]);
            //Destroy(shapes[index].gameObject);
            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];
            shapes.RemoveAt(lastIndex);
        }
    }



    public override void Save(GameDataWriter writer)
    {
        writer.Write(shapes.Count);
        writer.Write(loadedLevelBuildIndex);

        for (int i = 0; i < shapes.Count; i++)
        {
            print("Saving " + shapes[i].ShapeId + ", " + shapes[i].MaterialId);
            writer.Write(shapes[i].ShapeId);
            writer.Write(shapes[i].MaterialId);
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        if (version > saveVersion)
        {
            Debug.LogError("Unsupported future save version " + version);
            return;
        }
        int count = version <= 0 ? -version : reader.ReadInt();
        /* // The above line is a shorter version of the code in these comments
        int version = -reader.ReadInt();
		int count;
		if (version <= 0)
        {
			count = -version;
		}
		else
        {
			count = reader.ReadInt();
		}
        */

        StartCoroutine(LoadLevel(version < 2 ? 1 : reader.ReadInt()));

        for (int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            //int shapeId = reader.ReadInt();
            Shape instance = shapeFactory.Get(shapeId, materialId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }

}
