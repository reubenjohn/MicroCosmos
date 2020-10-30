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
        var childOrganelleTransform = child.transform.Find("Organelles");
        foreach (Transform childTransform in transform.Find("Organelles"))
        {
            var organelle = childTransform.GetComponent<IOrganelle>();
            if (organelle != null)
            {
                IGeneTranscriber geneTranscriber = organelle.GetGeneTranscriber();
                object inheritedGene = geneTranscriber.Mutate(organelle.GetGene());
                GameObject childOrganelleObject = (GameObject)Instantiate(organelle.LoadResource(), childOrganelleTransform);
                IOrganelle childOrganelle = childOrganelleObject.GetComponent<IOrganelle>();
                childOrganelle.OnInheritGene(inheritedGene);
            }
        }
    }

    public void OnSave(JsonWriter writer, JsonSerializer serializer)
    {
        var dict = new Dictionary<string, object>();
        dict.Add("position", Serialization.ToSerializable(transform.position));
        dict.Add("rotation", Serialization.ToSerializable(transform.rotation));
        IOrganelle[] organelles = transform.Find("Organelles").GetComponentsInChildren<IOrganelle>();
        dict.Add("organelles", organelles.Select(org => OrganelleUtils.ToSerializable(org)).ToArray());
        serializer.Serialize(writer, dict);
    }

    internal void OnLoad(JsonReader reader, JsonSerializer serializer)
    {
        var dict = serializer.Deserialize<Dictionary<string, JToken>>(reader);
        transform.position = dict.TryGetValue("position", out var v) ? Serialization.ToVector2(v.ToObject<float[]>()) : new Vector2();
        transform.rotation = dict.TryGetValue("rotation", out var q) ? Serialization.ToQuaternion(q.ToObject<float[]>()) : new Quaternion();
        Dictionary<string, object>[] organelleInfos = dict.TryGetValue("organelles", out var o) ? o.ToObject<Dictionary<string, object>[]>() : new Dictionary<string, object>[] { };
        Transform organellesTransform = transform.Find("Organelles");
        // IOrganelle[] organelles = organellesTransform.GetComponentsInChildren<IOrganelle>();
        foreach (Dictionary<string, object> organelleInfo in organelleInfos)
        {
            // dict.Add("organelles", organelles.Select(org => org.GetState().ToArray()));
        }
    }
}
