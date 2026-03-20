using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableLoader : MonoBehaviour
{
    public static AddressableLoader Instance { get; private set; }

    [SerializeField] private bool skipDownloadInEditor = true;

    public event Action<float> OnDownloadProgress;
    public event Action<string> OnStatusChanged;
    public event Action OnDownloadComplete;
    public event Action<string> OnDownloadFailed;

    public AudioClip LoadedMusicClip { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartLoading()
    {
        StartCoroutine(LoadSequence());
    }

    private IEnumerator LoadSequence()
    {
#if UNITY_ANDROID
        OnStatusChanged?.Invoke("Loading...");
        OnDownloadProgress?.Invoke(1f);
        yield return new WaitForSeconds(0.3f);
        OnStatusChanged?.Invoke("Ready!");
        OnDownloadComplete?.Invoke();
        yield break;
#endif

        OnStatusChanged?.Invoke("Checking connection...");
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            OnDownloadFailed?.Invoke("No internet connection");
            yield break;
        }

        OnStatusChanged?.Invoke("Initializing...");
        bool initSuccess = false;
        var initOp = Addressables.InitializeAsync();
        initOp.Completed += handle => { initSuccess = handle.Status == AsyncOperationStatus.Succeeded; };
        yield return initOp;

        if (!initSuccess)
        {
            OnDownloadFailed?.Invoke("Failed to initialize");
            yield break;
        }

        OnStatusChanged?.Invoke("Checking for updates...");
        bool catalogCheckSuccess = false;
        List<string> catalogsToUpdate = null;
        var checkOp = Addressables.CheckForCatalogUpdates(false);
        checkOp.Completed += handle =>
        {
            catalogCheckSuccess = handle.Status == AsyncOperationStatus.Succeeded;
            if (catalogCheckSuccess && handle.Result != null)
                catalogsToUpdate = new List<string>(handle.Result);
        };
        yield return checkOp;

        if (!catalogCheckSuccess)
        {
            OnDownloadFailed?.Invoke("Failed to check for updates");
            yield break;
        }

        if (catalogsToUpdate != null && catalogsToUpdate.Count > 0)
        {
            OnStatusChanged?.Invoke("Updating catalogs...");
            bool updateSuccess = false;
            var updateOp = Addressables.UpdateCatalogs(catalogsToUpdate, false);
            updateOp.Completed += handle => { updateSuccess = handle.Status == AsyncOperationStatus.Succeeded; };
            yield return updateOp;

            if (!updateSuccess)
            {
                OnDownloadFailed?.Invoke("Failed to update catalogs");
                yield break;
            }
        }

        OnStatusChanged?.Invoke("Checking download size...");
        bool sizeCheckSuccess = false;
        long downloadSize = 0;
        var sizeOp = Addressables.GetDownloadSizeAsync("music");
        sizeOp.Completed += handle =>
        {
            sizeCheckSuccess = handle.Status == AsyncOperationStatus.Succeeded;
            if (sizeCheckSuccess)
                downloadSize = handle.Result;
        };
        yield return sizeOp;

        if (!sizeCheckSuccess)
        {
            OnDownloadFailed?.Invoke("Failed to check download size");
            yield break;
        }

        if (downloadSize > 0)
        {
            OnStatusChanged?.Invoke("Downloading music...");
            bool downloadSuccess = false;
            var downloadOp = Addressables.DownloadDependenciesAsync("music", false);
            downloadOp.Completed += handle => { downloadSuccess = handle.Status == AsyncOperationStatus.Succeeded; };

            while (!downloadOp.IsDone)
            {
                OnDownloadProgress?.Invoke(downloadOp.PercentComplete);
                yield return null;
            }

            Addressables.Release(downloadOp);

            if (!downloadSuccess)
            {
                OnDownloadFailed?.Invoke("Failed to download music");
                yield break;
            }
        }

        OnStatusChanged?.Invoke("Loading music...");
        bool loadSuccess = false;
        var loadOp = Addressables.LoadAssetAsync<AudioClip>("GameMusic");
        loadOp.Completed += handle =>
        {
            loadSuccess = handle.Status == AsyncOperationStatus.Succeeded;
            if (loadSuccess)
                LoadedMusicClip = handle.Result;
        };
        yield return loadOp;

        if (!loadSuccess)
        {
            OnDownloadFailed?.Invoke("Failed to load music");
            yield break;
        }

        OnDownloadProgress?.Invoke(1f);
        OnStatusChanged?.Invoke("Ready!");
        OnDownloadComplete?.Invoke();
    }
}