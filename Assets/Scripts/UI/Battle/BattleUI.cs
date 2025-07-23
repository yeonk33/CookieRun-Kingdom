using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private Transform _dmgPool;
    [SerializeField] private float _textTime = 1f;

    private List<TMP_Text> dmgTxts;
    private Camera _camera;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        _camera = Camera.main;
        dmgTxts = _dmgPool.GetComponentsInChildren<TMP_Text>(true).ToList();
    }

    public void ShowDamage(int damage, Vector3 position, bool isCritical)
    {
        if (dmgTxts.Count == 0) return;
        TMP_Text txt = dmgTxts.Where(x => !x.gameObject.activeSelf).First();
        if (txt == null) return;
        txt.text = damage.ToString();
        txt.transform.position = _camera.WorldToScreenPoint(position);
        txt.gameObject.SetActive(true);

        StartCoroutine(ShowDamageText(txt));
    }

    private IEnumerator ShowDamageText(TMP_Text txt)
    {
        yield return new WaitForSeconds(_textTime);
        txt.gameObject.SetActive(false);
    }
}
