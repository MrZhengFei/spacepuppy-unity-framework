﻿using System;
using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.SPInput.Unity
{

    public static class SPInputDirect
    {

        #region Fields

        private const int ID_AXISLOW = (int)SPInputId.Axis1;
        private const int ID_AXISMID = (int)SPInputId.Axis28;
        private const int ID_AXISHIGH = (int)SPInputId.MouseAxis3;
        private const int ID_BUTTONLOW = (int)SPInputId.Button0;
        private const int ID_BUTTONHIGH = (int)SPInputId.MouseButton6;

        private static Dictionary<int, string> _inputIdToName;
        private static UnityEngine.KeyCode[] _allKeyCodes;

        #endregion

        #region SPInputId Extension Methods

        public static bool IsJoyAxis(this SPInputId id)
        {
            return id >= SPInputId.Axis1 && id <= SPInputId.Axis28;
        }

        public static bool IsMouseAxis(this SPInputId id)
        {
            return id >= SPInputId.MouseAxis1 && id <= SPInputId.MouseAxis3;
        }

        public static bool IsAxis(this SPInputId id)
        {
            return id >= SPInputId.Axis1 && id <= SPInputId.MouseAxis3;
        }

        public static bool IsJoyButton(this SPInputId id)
        {
            return id >= SPInputId.Button0 && id <= SPInputId.Button19;
        }

        public static bool IsMouseButton(this SPInputId id)
        {
            return id >= SPInputId.MouseButton0 && id <= SPInputId.MouseButton6;
        }

        public static bool IsButton(this SPInputId id)
        {
            return id >= SPInputId.Button0 && id <= SPInputId.MouseButton6;
        }

        #endregion

        #region Standard Input Testing

        public static bool GetButton(SPInputId button, Joystick joystick = Joystick.All)
        {
            return UnityEngine.Input.GetButton(GetInputName(button, joystick));
        }

        public static bool GetButtonDown(SPInputId button, Joystick joystick = Joystick.All)
        {
            return UnityEngine.Input.GetButtonDown(GetInputName(button, joystick));
        }

        public static bool GetButtonUp(SPInputId button, Joystick joystick = Joystick.All)
        {
            return UnityEngine.Input.GetButtonUp(GetInputName(button, joystick));
        }

        public static float GetAxis(SPInputId axis, Joystick joystick = Joystick.All)
        {
            return UnityEngine.Input.GetAxis(GetInputName(axis, joystick));
        }

        public static float GetAxisRaw(SPInputId axis, Joystick joystick = Joystick.All)
        {
            return UnityEngine.Input.GetAxisRaw(GetInputName(axis, joystick));
        }

        #endregion

        #region Polling

        public static SPInputId PollButton(Joystick joystick = Joystick.All)
        {
            if (joystick != Joystick.None)
            {
                for (int i = ID_BUTTONLOW; i <= ID_BUTTONHIGH; i++)
                {
                    if (UnityEngine.Input.GetButton(GetInputName((SPInputId)i, joystick)))
                    {
                        return (SPInputId)i;
                    }
                }
            }
            return SPInputId.Unknown;
        }

        public static bool TryPollButton(out SPInputId button, Joystick joystick = Joystick.All)
        {
            if (joystick != Joystick.None)
            {
                for (int i = ID_BUTTONLOW; i <= ID_BUTTONHIGH; i++)
                {
                    if (UnityEngine.Input.GetButton(GetInputName((SPInputId)i, joystick)))
                    {
                        button = (SPInputId)i;
                        return true;
                    }
                }
            }
            button = SPInputId.Unknown;
            return false;
        }

        public static SPInputId PollAxis(Joystick joystick = Joystick.All, bool pollMouse = false, float deadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            if (joystick != Joystick.None)
            {
                int high = pollMouse ? ID_AXISHIGH : ID_AXISMID;
                for (int i = ID_AXISLOW; i <= high; i++)
                {
                    float v = Input.GetAxis(GetInputName((SPInputId)i, joystick));
                    if (Mathf.Abs(v) > deadZone)
                    {
                        return (SPInputId)i;
                    }
                }
            }
            return SPInputId.Unknown;
        }

        public static bool TryPollAxis(out SPInputId axis, Joystick joystick = Joystick.All, bool pollMouse = false, float deadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            if (joystick != Joystick.None)
            {
                int high = pollMouse ? ID_AXISHIGH : ID_AXISMID;
                for (int i = ID_AXISLOW; i <= high; i++)
                {
                    float v = Input.GetAxis(GetInputName((SPInputId)i, joystick));
                    if (Mathf.Abs(v) > deadZone)
                    {
                        axis = (SPInputId)i;
                        return true;
                    }
                }
            }
            axis = SPInputId.Unknown;
            return false;
        }

        public static bool TryPollAxis(out SPInputId axis, out float value, Joystick joystick = Joystick.All, bool pollMouse = false, float deadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            if (joystick != Joystick.None)
            {
                int high = pollMouse ? ID_AXISHIGH : ID_AXISMID;
                for (int i = ID_AXISLOW; i <= high; i++)
                {
                    float v = Input.GetAxis(GetInputName((SPInputId)i, joystick));
                    if (Mathf.Abs(v) > deadZone)
                    {
                        axis = (SPInputId)i;
                        value = v;
                        return true;
                    }
                }
            }
            axis = SPInputId.Unknown;
            value = 0f;
            return false;
        }

        public static SPInputId[] PollAllAxes(Joystick joystick = Joystick.All, bool pollMouse = false)
        {
            if (joystick != Joystick.None)
            {
                using (var lst = com.spacepuppy.Collections.TempCollection.GetList<SPInputId>())
                {
                    int high = pollMouse ? ID_AXISHIGH : ID_AXISMID;
                    for (int i = ID_AXISLOW; i < high; i++)
                    {
                        if (Mathf.Abs(UnityEngine.Input.GetAxis(GetInputName((SPInputId)i, joystick))) > 0.5f)
                        {
                            lst.Add((SPInputId)i);
                        }
                    }

                    return lst.Count > 0 ? lst.ToArray() : com.spacepuppy.Utils.ArrayUtil.Empty<SPInputId>();
                }
            }

            return com.spacepuppy.Utils.ArrayUtil.Empty<SPInputId>();
        }

        public static UnityEngine.KeyCode PollKey()
        {
            if (_allKeyCodes == null) _allKeyCodes = System.Enum.GetValues(typeof(UnityEngine.KeyCode)) as UnityEngine.KeyCode[];

            for (int i = 0; i < _allKeyCodes.Length; i++)
            {
                if (UnityEngine.Input.GetKey(_allKeyCodes[i])) return _allKeyCodes[i];
            }
            return UnityEngine.KeyCode.None;
        }

        public static bool TryPollKey(out UnityEngine.KeyCode key)
        {
            if (_allKeyCodes == null) _allKeyCodes = System.Enum.GetValues(typeof(UnityEngine.KeyCode)) as UnityEngine.KeyCode[];

            key = KeyCode.None;
            for (int i = 0; i < _allKeyCodes.Length; i++)
            {
                if (UnityEngine.Input.GetKey(_allKeyCodes[i]))
                {
                    key = _allKeyCodes[i];
                    return true;
                }
            }
            return false;
        }

        #endregion


        #region ID Lookup

        public static string GetInputName(SPInputId id, Joystick joystick = Joystick.All)
        {
            if (id == SPInputId.Unknown) return null;

            if (_inputIdToName == null) _inputIdToName = new Dictionary<int, string>();
            int hash = (int)joystick | ((int)id << 4);
            string sname;
            if (_inputIdToName.TryGetValue(hash, out sname))
                return sname;
            
            if (joystick == Joystick.All)
            {
                if(id.IsJoyAxis())
                    sname = string.Format("JoyAll-Axis{0:00}", (int)id);
                else if(id.IsMouseAxis())
                    sname = string.Format("MouseAxis{0:0}", (int)id - (int)SPInputId.MouseAxis1 + 1);
                else if (id.IsJoyButton())
                    sname = string.Format("JoyAll-Button{0:00}", (int)id - (int)SPInputId.Button0);
                else if(id.IsMouseButton())
                    sname = string.Format("MouseButton{0:0}", (int)id - (int)SPInputId.MouseButton0);
            }
            else
            {
                if (id.IsJoyAxis())
                    sname = string.Format("Joy{0:0}-Axis{1:00}", (int)joystick, (int)id);
                else if (id.IsMouseAxis())
                    sname = string.Format("MouseAxis{0:0}", (int)id - (int)SPInputId.MouseAxis1 + 1);
                else if (id.IsJoyButton())
                    sname = string.Format("Joy{0:0}-Button{1:00}", (int)joystick, (int)id - (int)SPInputId.Button0);
                else if (id.IsMouseButton())
                    sname = string.Format("MouseButton{0:0}", (int)id - (int)SPInputId.MouseButton0);
            }

            _inputIdToName[hash] = sname;
            return sname;
        }
        
        #endregion

    }

}
