using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SoundwaveController : MonoBehaviour
{
    enum LaunchConfigs{Clean, Messy, Strange};

    [Tooltip("Turns on live adjustments; to be used for testing new configs")]
    [SerializeField] private bool experimentationMode;

    [Header("General Settings")]
    [Tooltip("Base particle system")]
    [SerializeField] private ParticleSystem waveGenerator;
    [Tooltip("Lifetime of the wave (seconds)")]
    [SerializeField] private float lifetime = 2f;
    [Tooltip("How much of the circle's circumference is used to send out the wave (0-360)")]
    [SerializeField] private float emissionRadius = 90f;

    [Header("Velocity Settings")]
    [Tooltip("Initial velocity of the wave")]
    [SerializeField] private float initialSpeed = 5f;            
    [Tooltip("What % (0-1) of the particle velocity is lost on collision")]
    [SerializeField] private float dampenRate = .2f;

    [Header("Visual Settings")]
    [Tooltip("Initial color of the wave")]
    [SerializeField] private Color color = Color.blue;

    [Header("Launch Configurations")]
    [Tooltip("Different particle size and wave pattern presets")]
    [SerializeField] private LaunchConfigs config;

    private void Start()
    {
        UpdateParticleSettings();
    }

    private void Update()
    {
        if (experimentationMode)
        {
            UpdateParticleSettings(); 
        }
    }

    private void UpdateParticleSettings()
    {
        //TODO: Swapping between two constants and one constant is Not Working
        var shapeVariables = waveGenerator.shape;
        var mainVariables = waveGenerator.main;
        var mainVariablesSize = waveGenerator.main.startSize;
        var collisionVariables = waveGenerator.collision;
        var noiseVariables = waveGenerator.noise;

        shapeVariables.arc = this.emissionRadius;
        mainVariables.startSpeed = this.initialSpeed;
        collisionVariables.dampen = this.dampenRate;
        mainVariables.startColor = this.color;

        if (config == LaunchConfigs.Clean)
        {
            //mainVariablesSize.mode = ParticleSystemCurveMode.Constant;
            mainVariablesSize.constantMin = .2f;
            mainVariablesSize.constantMax = .2f;
            mainVariables.startSize = new ParticleSystem.MinMaxCurve(0.2f, .2f);
            noiseVariables.enabled = false;
        }
        else if (config == LaunchConfigs.Messy)
        {
           // mainVariablesSize.mode = ParticleSystemCurveMode.TwoConstants;
            mainVariablesSize.constantMin = .06f;
            mainVariablesSize.constantMax = 1f;
            mainVariables.startSize = new ParticleSystem.MinMaxCurve(0.06f, .75f);
            noiseVariables.enabled = false;
        }
        else if (config == LaunchConfigs.Strange)
        {
           // mainVariablesSize.mode = ParticleSystemCurveMode.TwoConstants;
            mainVariablesSize.constantMin = .06f;
            mainVariablesSize.constantMax = 1f;
            mainVariables.startSize = new ParticleSystem.MinMaxCurve(0.06f, 1.0f);



            noiseVariables.enabled = true;
        }
    }


}
