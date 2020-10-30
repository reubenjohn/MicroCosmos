using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Cell : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GiveBirth()
    {
        // Regex regex = new Regex(@"(a-zA-Z0-9_)\[[0-9]+\]");
        // Group match = regex.Match(bodyPart.name).Groups[1];
        // match.Success
        Debug.Log("Giving birth");
        GameObject child = (GameObject)Instantiate(Resources.Load("Cells/Cell1"), transform.position - transform.up * .25f, transform.rotation);
        foreach (Transform childTransform in transform)
        {
            var organelle = childTransform.GetComponent<IOrganelle>();
            if (organelle != null)
            {
                IGeneTranscriber geneTranscriber = organelle.GetGeneTranscriber();
                object inheritedGene = geneTranscriber.Mutate(organelle.GetGene());
                var childOrganelleObject = (GameObject)Instantiate(organelle.LoadResource(), child.transform);
                var childOrganelle = childOrganelleObject.GetComponent<IOrganelle>();
                childOrganelle.OnInheritGene(inheritedGene);
            }
        }
    }

    public static float[] ToSerializable(Vector2 vec) => new float[] { vec.x, vec.y };
    public static float[] ToSerializable(Quaternion rot) => new float[] { rot.x, rot.y, rot.z, rot.w };

    public void OnSave(JsonWriter writer, JsonSerializer serializer)
    {
        var dict = new Dictionary<string, object>();
        dict.Add("position", ToSerializable(transform.position));
        dict.Add("rotation", ToSerializable(transform.rotation));
        IOrganelle[] organelles = transform.Find("Organelles").GetComponentsInChildren<IOrganelle>();
        dict.Add("organelles", organelles.Select(org => ToSerializable(org)).ToArray());
        serializer.Serialize(writer, dict);
    }

    private Dictionary<string, object> ToSerializable(IOrganelle organelle)
    {
        var organelleInfo = new Dictionary<string, object>();
        Type organelleType = organelle.GetGeneTranscriber().GetType();
        organelleInfo["type"] = organelleType.FullName;
        organelleInfo["state"] = organelle.GetState();
        return organelleInfo;
    }

    internal void OnLoad(JsonReader reader, JsonSerializer serializer)
    {
        var dict = serializer.Deserialize<Dictionary<string, JToken>>(reader);
        transform.position = dict.TryGetValue("position", out var v) ? ToVector2(v.ToObject<float[]>()) : new Vector2();
        transform.rotation = dict.TryGetValue("rotation", out var q) ? ToQuaternion(q.ToObject<float[]>()) : new Quaternion();
        Dictionary<string, object>[] organelleInfos = dict.TryGetValue("organelles", out var o) ? o.ToObject<Dictionary<string, object>[]>() : new Dictionary<string, object>[] { };
        Transform organellesTransform = transform.Find("Organelles");
        // IOrganelle[] organelles = organellesTransform.GetComponentsInChildren<IOrganelle>();
        foreach (Dictionary<string, object> organelleInfo in organelleInfos)
        {
            // dict.Add("organelles", organelles.Select(org => org.GetState().ToArray()));
        }
    }

    private Quaternion ToQuaternion(float[] vs)
    {
        return new Quaternion(vs[0], vs[1], vs[2], vs[3]);
    }

    private Vector2 ToVector2(float[] vs)
    {
        return new Vector2(vs[0], vs[1]);
    }

    public void Load()
    {

        // string filePath = Application.persistentDataPath + "/dna.json";
        // FileStream file = File.Open(filePath, FileMode.Open);
        // StreamReader sr = new StreamReader(file);
        // Dna dna = (Dna)JsonUtility.FromJson<Dna>(sr.ReadToEnd());
        // file.Close();

        // Debug.Log("DNA loaded from " + filePath);
    }
}
