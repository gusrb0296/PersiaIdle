using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DataManager
{
    private static DataManager _instance;

    public static DataManager Instance
    {
        get
        {
            if (ReferenceEquals(_instance, null))
                _instance = new DataManager();
            return _instance;
        }
    }

    private Dictionary<string, ES3Settings> saveSettings;

    public DataManager()
    {
        saveSettings = new Dictionary<string, ES3Settings>();
    }

    public void Save<T>(string id, T data)
    {
        if (saveSettings.TryGetValue(id, out var saveSetting))
            ES3.Save<T>(id, data, saveSetting);
        else
        {
            ES3Settings setting = new ES3Settings();
            saveSettings.Add(id, setting);
            ES3.Save<T>(id, data, setting);
        }
    }

    public void Save<T>(string id, T data, ES3Settings setting)
    {
        saveSettings.TryAdd(id, setting);
        ES3.Save<T>(id, data, setting);
    }

    public T Load<T>(string id, T defaultValue)
    {
        if (saveSettings.TryGetValue(id, out var saveSetting))
        {
            return ES3.Load<T>(id, defaultValue, saveSetting);
        }
        else
        {
            ES3Settings setting = new ES3Settings();
            saveSettings.Add(id, setting);
            return ES3.Load<T>(id, defaultValue, setting);
        }
    }

    public T Load<T>(string id)
    {
        if (saveSettings.TryGetValue(id, out var saveSetting))
        {
            return ES3.Load<T>(id, saveSetting);
        }
        else
        {
            ES3Settings setting = new ES3Settings();
            saveSettings.Add(id, setting);
            return ES3.Load<T>(id, setting);
        }
    }

    public T Load<T>(string id, T defaultValue, ES3Settings setting)
    {
        saveSettings.TryAdd(id, setting);
        return ES3.Load<T>(id, defaultValue, setting);
    }
}
