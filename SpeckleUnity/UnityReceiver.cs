using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using Newtonsoft.Json;

using System.Reflection;

public class UnityReceiver : MonoBehaviour
{
    private SpeckleCore.SpeckleApiClient Client;

    private List<SpeckleCore.SpeckleObject> SpeckleObjects;
    private Dictionary<SpeckleCore.Layer, GameObject> Layers = new Dictionary<SpeckleCore.Layer, GameObject>();
    
    private bool bRefreshDisplay = false, bRefreshingDisplay = false;

    private GameObject rootGameObject;
    
    private string authToken = "put auth token here"; //TODO - actually login to get this

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (bRefreshDisplay && !bRefreshingDisplay)
        {
            //Clears existing objects first
            bRefreshDisplay = false;
            bRefreshingDisplay = true;
            UpdateGlobal();
        }

    }

    // Initialise receiver
    public async void Init(string StreamId, string URL) 
    {
        Client = new SpeckleCore.SpeckleApiClient(URL);

        // Assign events
        Client.OnReady += Client_OnReady;
        Client.OnLogData += Client_OnLogData;
        Client.OnWsMessage += Client_OnWsMessage;
        Client.OnError += Client_OnError;

        SpeckleObjects = new List<SpeckleCore.SpeckleObject>();

        await Client.IntializeReceiver(StreamId, Application.productName, "Unity", Application.buildGUID, authToken);

        Client.Stream = (await Client.StreamGetAsync(Client.StreamId, null)).Resource;

        rootGameObject = new GameObject(Client.Stream.Name);

        // Call event on SpeckleManager to allow users to do their own thing when a stream is created
        transform.GetComponent<UnitySpeckle>().OnReceiverCreated.Invoke(this);

        UpdateGlobal();
    }

    public async void UpdateGlobal()
    {
        bRefreshingDisplay = true;

        var streamGetResponse = await Client.StreamGetAsync(Client.StreamId, null);

        if (streamGetResponse.Success == false)
        {
            Debug.Log(streamGetResponse.Message); // TODO: Actually handle this
        }

        Client.Stream = streamGetResponse.Resource; // This is gross of the C# API
        
        Debug.Log("Getting objects....");
        var getObjectResult = await Client.ObjectGetBulkAsync(Client.Stream.Objects.Select(obj => obj._id).ToArray(), null); // TODO: Check if ObjectGetBulkAsync returns objects in a corresponding order. If it does, the next little bit can be simplified
        
        Dictionary<string, SpeckleCore.SpeckleObject> ObjectCache = getObjectResult.Resources.ToDictionary(speckleObject => speckleObject._id);
        
        SpeckleObjects = Client.Stream.Objects.Select(speckleObject => ObjectCache[speckleObject._id]).ToList();

        // Remove old layers
        foreach (SpeckleCore.Layer layer in Layers.Keys.Where(layer => Client.Stream.Layers.Where(newLayer => newLayer.Guid != layer.Guid).Count() > 0).ToList()) // ToList is required because we're modifying Layers, I think
        {
            Destroy(Layers[layer]);
            Layers.Remove(layer);
        }

        // Add new layers
        foreach (SpeckleCore.Layer layer in Client.Stream.Layers.Where(layer => !Layers.ContainsKey(layer)).ToList())
        {
            Layers[layer] = new GameObject(layer.Name);
            Layers[layer].transform.SetParent(rootGameObject.transform);
        }

        // Mark all existing objects
        foreach (UnitySpeckleObjectData usod in rootGameObject.GetComponentsInChildren<UnitySpeckleObjectData>())
        {
            usod.LayerName = null;
        }

        // Create new objects
        for (int i = 0; i < SpeckleObjects.Count; i++)
        {
            SpeckleCore.Layer layer = LayerFromIndex(i);
            GameObject layerObject = Layers[layer];
            GameObject gameObject = (GameObject)SpeckleCore.Converter.Deserialise(SpeckleObjects[i]); // TODO: This creates a new game object even if one already exists. It should reuse objects which already exist
            // TODO: The more fundamental issue is that checking which layer an object is in is tricky. Perhaps layers map more naturally to Unity's tags.

            gameObject.GetComponent<UnitySpeckleObjectData>().LayerName = layer.Name;
            gameObject.transform.SetParent(layerObject.transform);
        }

        // Destroy all objects which are still marked
        foreach (UnitySpeckleObjectData usod in rootGameObject.GetComponentsInChildren<UnitySpeckleObjectData>()) if (usod.LayerName == null)
        {
            Destroy(usod.gameObject);
        }

        bRefreshingDisplay = false;
        transform.GetComponent<UnitySpeckle>().OnUpdateReceived.Invoke(this);
    }

    private SpeckleCore.Layer LayerFromIndex(int index)
    {
        return Client.Stream.Layers.FirstOrDefault(layer => layer.StartIndex <= index && index < layer.StartIndex + layer.ObjectCount);
    }

    public virtual void Client_OnReady(object source, SpeckleCore.SpeckleEventArgs e)
    {
        Debug.Log("Client ready");
        Debug.Log(JsonConvert.SerializeObject(e.EventData));
    }
    public virtual void Client_OnLogData(object source, SpeckleCore.SpeckleEventArgs e)
    {
        Debug.Log("Client LogData");
        Debug.Log(JsonConvert.SerializeObject(e.EventData));
    }
    public virtual void Client_OnWsMessage(object source, SpeckleCore.SpeckleEventArgs e)
    {
        Debug.Log("Client WsMessage");
        Debug.Log(JsonConvert.SerializeObject(e.EventData));

        bRefreshDisplay = true;
        
    }
    public virtual void Client_OnError(object source, SpeckleCore.SpeckleEventArgs e)
    {
        Debug.LogError(JsonConvert.SerializeObject(e.EventData));
    }

    void OnDestroy()
    {
        Client.Dispose(true);
    }
}
