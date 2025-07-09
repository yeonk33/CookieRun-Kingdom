using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private Dictionary<UIType, GameObject> _uis = new Dictionary<UIType, GameObject>();
    private Transform _canvas;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        _canvas = GameObject.Find("Canvas").transform;

        LoadAllPrefabs();
    }

    private void LoadAllPrefabs()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/UI");
        foreach (GameObject p in prefabs)
        {
            GameObject go = Instantiate(p, _canvas);

            var panelUI = go.GetComponent<IPanelUI>();
            if (panelUI == null)
            {
                Debug.LogWarning($"{p.name} IPanelUI interface 구현 안됨");
                Destroy(go);
                continue;
            }

            UIType type = panelUI.Type;
            if (_uis.ContainsKey(type))
            {
                Debug.LogWarning($"{type} UI 이미 존재함");
                Destroy(go);
                continue;
            }

            _uis.Add(type, go);
            go.SetActive(false);
            Debug.Log($"{_uis[type].name} UI 로드 완료");
        }
    }

    public void ShowUI(UIType type)
    {
        if (_uis.TryGetValue(type, out GameObject ui))
        {
            ui.SetActive(true);
            Debug.Log($"{ui.name} UI 활성화");
        }
        else
        {
            Debug.LogWarning($"{type} UI가 존재하지 않음");
        }
    }

    public void HideUI(UIType type)
    {
        if (_uis.TryGetValue(type, out GameObject ui))
        {
            ui.SetActive(false);
            Debug.Log($"{ui.name} UI 비활성화");
        }
        else
        {
            Debug.LogWarning($"{type} UI가 존재하지 않음");
        }
    }

    public GameObject GetUI(Define.UIType type)
    {
        GameObject ui;
        if (_uis.TryGetValue(type, out ui))
        {
            return ui;
        }
        else
        {
            Debug.LogWarning($"{type} UI가 존재하지 않음");
            ui = null;
            return null;
        }
    }
}
