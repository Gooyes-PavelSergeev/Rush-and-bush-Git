using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class ColorComponent : PlayerComponent
{
    [SerializeField] private List<ColorConfig> _colorConfigs;

    [SerializeField] private SkinnedMeshRenderer _targetRenderer;

    private void Start()
    {
        foreach (ColorConfig config in _colorConfigs)
        {
            if (config.colorType == ColorType.Normal) _targetRenderer.material = config.material;
            return;
        }
    }

    private void OnEnable()
    {
        _entity.BashComponent.OnBeingBashed += ChangeColor;
    }

    private void OnDisable()
    {
        _entity.BashComponent.OnBeingBashed -= ChangeColor;
    }

    private void ChangeColor(bool toBashState)
    {
        foreach (ColorConfig config in _colorConfigs)
        {
            if (toBashState)
            {
                if (config.colorType == ColorType.Bash)
                {
                    _entity.PlayerData.RegisterMaterial(config.id);
                    return;
                }
            }
            else
            {
                if (config.colorType == ColorType.Normal)
                {
                    _entity.PlayerData.RegisterMaterial(config.id);
                    return;
                } 
            }
        }
    }

    public void SetMaterial(int materialId)
    {
        foreach(ColorConfig config in _colorConfigs)
            if (config.id == materialId)
            {
                _targetRenderer.material = config.material;
                return;
            }
    }

    public override void OnRestart()
    {
        foreach (ColorConfig config in _colorConfigs)
            if (config.colorType == ColorType.Normal)
            {
                _entity.PlayerData.RegisterMaterial(config.id);
                return;
            }
    }

    [Serializable]
    private class ColorConfig
    {
        public int id;
        public Material material;
        public ColorType colorType;
    }

    private enum ColorType
    {
        Bash,
        Rush,
        Normal
    }
}
