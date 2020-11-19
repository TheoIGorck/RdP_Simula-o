using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoverView : MonoBehaviour
{
    [SerializeField]
    private Text _soldiersText = default;
    [SerializeField]
    private Text _ammo = default;

    [SerializeField]
    private Image _health = default;
    [SerializeField]
    private Image _fuel = default;
    [SerializeField]
    private Image _shield = default;
    [SerializeField]
    private Image _Emptyshield = default;
    [SerializeField]
    private Image _dijkstra = default;

    void Start()
    {
        GameObject.Find("Rover(Clone)").GetComponent<Rover>().OnRoverStatusChanged += OnRoverStatusChanged;
    }
    
    void Update()
    {
        
    }

    private void OnRoverStatusChanged(object sender, RoverStatusArgs args)
    {
        _ammo.text = args.Ammo.ToString();
        _health.fillAmount = args.Health / 30f;
        _fuel.fillAmount = args.Fuel / 100f;
        _soldiersText.text = args.RescuedSoldiers.ToString();
        _shield.gameObject.SetActive(args.Shield);
        _Emptyshield.gameObject.SetActive(args.EmptyShield);
        _dijkstra.gameObject.SetActive(args.Dijkstra);
    }
}
