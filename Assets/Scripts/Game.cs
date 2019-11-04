/*
using System.Collections;
using System.Collections.Generic;

using System.IO; // Input/Output namespace

using UnityEngine;

public class Game : MonoBehaviour
{
    public Transform prefab;
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;

    public float radius;
    public float minScale;
    public float maxScale;

    List<Transform> objects = new List<Transform>();
    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
        }

        if (Input.GetKey(createKey))
        {
            CreateObject();
        }

        if (Input.GetKeyDown(saveKey))
        {
            Save();
        }

        if (Input.GetKeyDown(loadKey))
        {
            Load();
        }
    }

    private void Save()
    {
        // BinaryWriter writer = new BinaryWriter(File.Open(savePath, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            writer.Write(objects.Count);
            for (int i = 0; i < objects.Count; i++)
            {
                Transform t = objects[i];
                writer.Write(t.localPosition.x);
                writer.Write(t.localPosition.y);
                writer.Write(t.localPosition.z);
            }
        }
    }

    void Load()
    {
        BeginNewGame();
        using (BinaryReader reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Vector3 p;
                p.x = reader.ReadSingle();
                p.y = reader.ReadSingle();
                p.z = reader.ReadSingle();
                Transform t = Instantiate(prefab);
                t.localPosition = p;
                objects.Add(t);
            }
        }
    }



    void BeginNewGame()
    {
        for(int i = 0; i < objects.Count; i++)
        {
            Destroy(objects[i].gameObject);
        }
        objects.Clear();
    }

    void CreateObject()
    {
        Transform t = Instantiate(prefab);
        t.localPosition = Random.insideUnitSphere * radius;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(minScale, maxScale);
        objects.Add(t); // This script runs afterwards so the object is only added once all its important data is set.
    }
}
*/


using System.Collections;
using System.Collections.Generic;

//using System.IO; // Input/Output namespace

using UnityEngine;

public class Game : PersistableObject
{
    public PersistableObject prefab;
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;

    public PersistentStorage storage;

    public float radius;
    public float minScale;
    public float maxScale;

    List<PersistableObject> objects = new List<PersistableObject>();
    private string savePath;

    private void Awake()
    {
        //savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
        }

        if (Input.GetKey(createKey))
        {
            CreateObject();
        }

        
        if (Input.GetKeyDown(saveKey))
        {
            storage.Save(this);
        }

        if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
        
    }
    /*
    private void Save()
    {
        // BinaryWriter writer = new BinaryWriter(File.Open(savePath, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            writer.Write(objects.Count);
            for (int i = 0; i < objects.Count; i++)
            {
                Transform t = objects[i];
                writer.Write(t.localPosition.x);
                writer.Write(t.localPosition.y);
                writer.Write(t.localPosition.z);
            }
        }
    }

    void Load()
    {
        BeginNewGame();
        using (BinaryReader reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Vector3 p;
                p.x = reader.ReadSingle();
                p.y = reader.ReadSingle();
                p.z = reader.ReadSingle();
                Transform t = Instantiate(prefab);
                t.localPosition = p;
                objects.Add(t);
            }
        }
    }
    */


    void BeginNewGame()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            Destroy(objects[i].gameObject);
        }
        objects.Clear();
    }

    void CreateObject()
    {
        /*
        Transform t = Instantiate(prefab);
        t.localPosition = Random.insideUnitSphere * radius;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(minScale, maxScale);
        objects.Add(t); // This script runs afterwards so the object is only added once all its important data is set.
        */

        PersistableObject o = Instantiate(prefab);
        Transform t = o.transform;
        t.localPosition = Random.insideUnitSphere * radius;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(minScale, maxScale);
        objects.Add(o); // This script runs afterwards so the object is only added once all its important data is set.
    }



    public override void Save(GameDataWriter writer)
    {
        writer.Write(objects.Count);
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            PersistableObject o = Instantiate(prefab);
            o.Load(reader);
            objects.Add(o);
        }
    }

}
