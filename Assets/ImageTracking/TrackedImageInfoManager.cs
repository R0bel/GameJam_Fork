using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

/// This component listens for images detected by the <c>XRImageTrackingSubsystem</c>
/// and overlays some information as well as the source Texture2D on top of the
/// detected image.
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImageInfoManager : MonoBehaviour, IManagedBehaviour
{
    GameManager gameManager;

    [SerializeField]
    private bool debugImageFound = false;

    [SerializeField]
    private ARLevel[] levels = new ARLevel[2];
    private ARTrackedImageManager m_TrackedImageManager;

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    public void OnStart(GameManager _manager)
    {
        gameManager = _manager;
        gameManager.Events.StartupFinished += OnGameStartupFinished;
    }

    private void OnGameStartupFinished()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
#if UNITY_EDITOR
        if (debugImageFound) ActivateLevel(levels[0]);
#endif
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    public ARLevel[] ARLevels
    {
        get
        {
            return levels;
        }
    }

    public ARLevel ActiveLevel { get; private set; }

    void UpdateTransform(ARTrackedImage trackedImage)
    {
        foreach (ARLevel level in levels)
        {
            if (level == null) continue;
            if (trackedImage.referenceImage.name == level.ImageName)
            {

                // set level active
                level.gameObject.transform.position = trackedImage.transform.position;
                level.gameObject.transform.rotation = trackedImage.transform.rotation;

                ActivateLevel(level);
                break;
            }
        }
    }

    private void ActivateLevel(ARLevel _level)
    {
        if (_level != ActiveLevel)
        {
            if (ActiveLevel != null) ActiveLevel.StopLevel();
            ActiveLevel = _level;
            Debug.Log("Start Level!");

            // set all level inactive
            Array.ForEach(levels, l => l.gameObject.SetActive(false));
            if (!_level.gameObject.activeSelf) _level.gameObject.SetActive(true);

            // start ARLevel
            _level.StartLevel();
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        gameManager.Events.OnTrackedImagesChanged(eventArgs);
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateTransform(trackedImage);
            // Info(trackedImage);
        }
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateTransform(trackedImage);
            // Info(trackedImage);
        }
    }

    void Info(ARTrackedImage trackedImage)
    {
        Debug.Log("trackedImage: " + trackedImage.transform);
        Debug.Log("referenceImage: " + trackedImage.referenceImage.name);
    }
}
