using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoverView : MonoBehaviour
{
    [SerializeField]
    private Text _ammoText = default;
    [SerializeField]
    private Text _healthText = default;
    [SerializeField]
    private Text _fuelText = default;
    [SerializeField]
    private Text _soldiersText = default;
    
    void Start()
    {
        GameObject.Find("Rover(Clone)").GetComponent<Rover>().OnRoverStatusChanged += OnRoverStatusChanged;
    }
    
    void Update()
    {
        
    }

    private void OnRoverStatusChanged(object sender, RoverStatusArgs args)
    {
        _ammoText.text = "Ammo: " + args.Ammo.ToString();
        _healthText.text = "Health: " + args.Health.ToString();
        _fuelText.text = "Fuel: " + args.Fuel.ToString();
        _soldiersText.text = "Rescued Soldiers: " + args.RescuedSoldiers.ToString();
    }
}
