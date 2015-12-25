﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MapLogicStructure : MapLogicObject
{
    public override MapLogicObjectType GetObjectType() { return MapLogicObjectType.Structure; }
    protected override Type GetGameObjectType() { return typeof(MapViewStructure); }

    public StructureClass Class = null;
    public Templates.TplStructure Template = null;
    public int CurrentFrame = 0;
    public int CurrentTime = 0;
    public int HealthMax = 0;
    public int Health = 0;
    public MapLogicPlayer Player = null;
    public bool IsBridge = false;
    public int Tag = 0;
    public float ScanRange = 0;

    public MapLogicStructure(int typeId)
    {
        Class = StructureClassLoader.GetStructureClassById(typeId);
        if (Class == null)
            Debug.Log(string.Format("Invalid structure created (typeId={0})", typeId));
        else InitStructure();
    }

    public MapLogicStructure(string name)
    {
        Class = StructureClassLoader.GetStructureClassByName(name);
        if (Class == null)
            Debug.Log(string.Format("Invalid structure created (name={0})", name));
        else InitStructure();
    }

    private void InitStructure()
    {
        Template = TemplateLoader.GetStructureById(Class.ID);
        if (Template == null)
        {
            Debug.Log(string.Format("Invalid structure created (template not found, typeId={0})", Class.ID));
            Class = null;
            return;
        }

        HealthMax = Health = Template.HealthMax;
        Width = Template.Width;
        Height = Template.Height;
        ScanRange = Template.ScanRange; // only default scanrange
        DoUpdateView = true;
    }

    public override void Update()
    {
        if (Class == null)
            return;
        // do not animate if visibility != 2, also do not render at all if visibility == 0
        if (Class.Frames.Length > 1 && GetVisibility() == 2)
        {
            CurrentTime++;
            if (CurrentTime > Class.Frames[CurrentFrame].Time)
            {
                CurrentFrame = ++CurrentFrame % Class.Frames.Length;
                CurrentTime = 0;
                DoUpdateView = true;
            }
        }
    }

    public override MapNodeFlags GetNodeLinkFlags(int x, int y)
    {
        if (IsBridge) return MapNodeFlags.Unblocked;

        bool canNotPass = ((1 << (y * Width + x)) & Template.CanNotPass) != 0;
        bool canPass = ((1 << (y * Width + x)) & Template.CanPass) != 0;
        if (canPass) return MapNodeFlags.Unblocked;
        if (canNotPass) return MapNodeFlags.BlockedGround;
        return 0;
    }
}