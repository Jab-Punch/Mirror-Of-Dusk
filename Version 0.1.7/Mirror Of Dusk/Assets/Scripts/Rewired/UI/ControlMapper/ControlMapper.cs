using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rewired.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
    public partial class ControlMapper : MonoBehaviour
    {
        [SerializeField] private bool _showControllerGroupButtons = false;


        #region Properties
        public bool showControllerGroupButtons { get { return _showControllerGroupButtons; } set { _showControllerGroupButtons = value; InspectorPropertyChanged(true); } }
        #endregion
        
        private void OnControlsChanged()
        {
            if (base.gameObject.activeInHierarchy)
            {
                this.Redraw(false, false);
            }
        }

        public ControlMapper.MappingSet[] GetMappingSet
        {
            get { return this._mappingSets; }
        }

        /*public void hy()
        {
            pendingInputMapping.map.ReplaceOrCreateElementMap();
        }*/

        #region Main Window Event Handlers
        private void ToggleRumble()
        {
            SettingsData.Data.canVibrate = !SettingsData.Data.canVibrate;
        }
        #endregion
    }
}