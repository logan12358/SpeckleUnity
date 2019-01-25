using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using Newtonsoft.Json;

using System.Reflection;

public class UnityReceiver : MonoBehaviour
{

    public SpeckleCore.SpeckleApiClient Client { get; set; }
    public List<SpeckleCore.SpeckleObject> SpeckleObjects { get; set; }
    public List<object> ConvertedObjects;
    
    private Dictionary<String, SpeckleCore.SpeckleObject> ObjectCache = new Dictionary<string, SpeckleCore.SpeckleObject>();
    private bool bUpdateDisplay = false;
    private bool bRefreshDisplay = false;

    public GameObject rootGameObject;
    
    private string authToken = "put auth token here"; //TODO - actually login to get this
    private string StreamID;

    

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (bUpdateDisplay)
        {
            //Initial creation of objects
            bUpdateDisplay = false;
            CreateObjects();

            //call event on SpeckleManager to allow users to do their own thing when a stream is updated
            transform.GetComponent<UnitySpeckle>().OnUpdateReceived.Invoke(this);
        }
        if (bRefreshDisplay)
        {
            //Clears existing objects first
            bRefreshDisplay = false;
            RefreshObjects();
        }

    }

    // Initialise receiver
    public async void Init(string inStreamID, string URL) 
    {
        Client = new SpeckleCore.SpeckleApiClient(URL);

        //Assign events
        Client.OnReady += Client_OnReady;
        Client.OnLogData += Client_OnLogData;
        Client.OnWsMessage += Client_OnWsMessage;
        Client.OnError += Client_OnError;

        SpeckleObjects = new List<SpeckleCore.SpeckleObject>();

        StreamID = inStreamID;
        await Client.IntializeReceiver(StreamID, Application.productName, "Unity", Application.buildGUID, authToken);

        Client.Stream = (await Client.StreamGetAsync(StreamID, null)).Resource;

        if (rootGameObject == null)
            rootGameObject = new GameObject(Client.Stream.Name);

        //call event on SpeckleManager to allow users to do their own thing when a stream is created
        transform.GetComponent<UnitySpeckle>().OnReceiverCreated.Invoke(this);

        UpdateGlobal();
    }

    //Wrapper for update coroutine
    public async void UpdateGlobal()
    {        
        SpeckleObjects.Clear();
        ObjectCache.Clear();
        var streamGetResponse = Client.StreamGetAsync(StreamID, null).Result;
        if (streamGetResponse.Success == false)
        {
            Debug.Log(streamGetResponse.Message);
        }

        Client.Stream = streamGetResponse.Resource;
        
        Debug.Log("Getting objects....");
        var payload = Client.Stream.Objects.Select(obj => obj._id).ToArray();
	
        var getObjectResult = await Client.ObjectGetBulkAsync(payload, null);
               
        foreach (var x in getObjectResult.Resources)
            ObjectCache.Add(x._id, x);
        
        foreach (var obj in Client.Stream.Objects)
            SpeckleObjects.Add(ObjectCache[obj._id]);             

        bUpdateDisplay = true;       
    }

    public void RefreshObjects()
    {
        //Clear existing objects
        //TODO - update existing objects instead of destroying/recreating all of them
        foreach (var co in ConvertedObjects)
        {
            GameObject tempObj = (GameObject)co;
            Destroy(tempObj);
        }
        ConvertedObjects.Clear();

        UpdateGlobal();        
    }

    public void CreateObjects()
    {
        //Generate native GameObjects with methods from SpeckleUnityConverter 
        ConvertedObjects = SpeckleCore.Converter.Deserialise(SpeckleObjects);
        
        for (int i = 0; i < ConvertedObjects.Count; i++)
        {
            GameObject go = (GameObject)ConvertedObjects[i];
            SpeckleCore.Layer layer = LayerFromIndex(i);
            GameObject layerObject = LayerGameObjectFromLayer(layer);

            go.GetComponent<UnitySpeckleObjectData>().LayerName = layer.Name;
            go.transform.SetParent(layerObject.transform);
        }

    }

    private SpeckleCore.Layer LayerFromIndex(int index)
    {
        return Client.Stream.Layers.FirstOrDefault(layer => layer.StartIndex <= index && index < layer.StartIndex + layer.ObjectCount);
    }

    private GameObject LayerGameObjectFromLayer(SpeckleCore.Layer layer)
    {
            GameObject layerObject = GameObject.Find(layer.Name);
            if (layerObject == null)
            {
                layerObject = new GameObject(layer.Name);
                layerObject.transform.SetParent(rootGameObject.transform);
            }
            return layerObject;
    }
    

    public virtual void Client_OnReady(object source, SpeckleCore.SpeckleEventArgs e)
    {
        Debug.Log("Client ready");
        //Debug.Log(JsonConvert.SerializeObject(e.EventData));
    }
    public virtual void Client_OnLogData(object source, SpeckleCore.SpeckleEventArgs e)
    {
        //Debug.Log("Client LogData");
        //Debug.Log(JsonConvert.SerializeObject(e.EventData));
    }
    public virtual void Client_OnWsMessage(object source, SpeckleCore.SpeckleEventArgs e)
    {
        //Debug.Log("Client WsMessage");
        //Debug.Log(JsonConvert.SerializeObject(e.EventData));

        //Set refresh to true to prompt recreating geometry
        bRefreshDisplay = true;
        
    }
    public virtual void Client_OnError(object source, SpeckleCore.SpeckleEventArgs e)
    {
        //Debug.Log("Client Error");
        //Debug.Log(JsonConvert.SerializeObject(e.EventData));
    }
}
