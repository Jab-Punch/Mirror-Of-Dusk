using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Rewired.Data.Mapping;

public class ControllerGlyphs : MonoBehaviour
{
    [SerializeField] private ControllerEntry[] controllers;
    [SerializeField] private TemplateEntry[] templates;
    private static ControllerGlyphs Instance;

    [Serializable]
    private class ControllerEntry
    {
        public string name;

        public HardwareJoystickMap joystick;
        public GlyphEntry[] glyphs;

        public Sprite GetGlyph(int elementIdentifierId, AxisRange axisRange)
        {
            if (glyphs == null) return null;
            for (int i = 0; i < glyphs.Length; i++)
            {
                if (glyphs[i] == null) continue;
                if (glyphs[i].elementIdentifierId != elementIdentifierId) continue;
                return glyphs[i].GetGlyph(axisRange);
            }
            return null;
        }
    }

    [Serializable]
    private class TemplateEntry
    {
        public string name;

        public HardwareJoystickTemplateMap joystick;
        public GlyphEntry[] glyphs;

        public Sprite GetGlyph(int elementIdentifierId, AxisRange axisRange)
        {
            if (glyphs == null) return null;
            for (int i = 0; i < glyphs.Length; i++)
            {
                if (glyphs[i] == null) continue;
                if (glyphs[i].elementIdentifierId != elementIdentifierId) continue;
                return glyphs[i].GetGlyph(axisRange);
            }
            return null;
        }
    }

    [Serializable]
    private class GlyphEntry
    {
        public int elementIdentifierId;
        public Sprite glyph;
        public Sprite glyphPos;
        public Sprite glyphNeg;

        public Sprite GetGlyph(AxisRange axisRange)
        {
            switch(axisRange)
            {
                case AxisRange.Full:
                    return glyph;
                case AxisRange.Positive:
                    return (glyphPos != null) ? glyphPos : glyph;
                case AxisRange.Negative:
                    return (glyphNeg != null) ? glyphNeg : glyph;
            }
            return null;
        }
    }

    private void Awake()
    {
        ControllerGlyphs.Instance = this;
    }

    public static Sprite GetGlyph(Joystick _joystick, int elementIdentifierId, AxisRange axisRange)
    {
        if (Instance == null) return null;
        if (Instance.controllers == null) return null;
        if (elementIdentifierId == -1) return null;

        for (int i = 0; i < Instance.controllers.Length; i++)
        {
            if (Instance.controllers[i] == null) continue;
            if (Instance.controllers[i].joystick == null) continue;
            if (Instance.controllers[i].joystick.Guid != _joystick.hardwareTypeGuid) continue;
            return Instance.controllers[i].GetGlyph(elementIdentifierId, axisRange);
        }

        for (int j = 0; j < _joystick.Templates.Count; j++)
        {
            for (int i = 0; i < Instance.templates.Length; i++)
            {
                if (Instance.templates[i] == null) continue;
                if (Instance.templates[i].joystick == null) continue;
                if (Instance.templates[i].joystick.Guid != _joystick.Templates[0].typeGuid) continue;
                return Instance.templates[i].GetGlyph(elementIdentifierId, axisRange);
            }
        }
        return null;
    }

    public static Sprite GetDefaultGlyph(int elementIdentifierId, AxisRange axisRange)
    {
        if (Instance == null) return null;
        if (Instance.controllers == null) return null;
        if (elementIdentifierId == -1) return null;

        System.Guid _guid = new System.Guid("83b427e4-086f-47f3-bb06-be266abd1ca5");

        for (int i = 0; i < Instance.templates.Length; i++)
        {
            if (Instance.templates[i] == null) continue;
            if (Instance.templates[i].joystick == null) continue;
            if (Instance.templates[i].joystick.Guid != _guid) continue;
            return Instance.templates[i].GetGlyph(elementIdentifierId, axisRange);
        }
        return null;
    }
}
