using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paps.Cheats
{
    public class TestCheatOverlay : ICheatOverlay
    {
        public string Id => "test-cheat-overlay";

        public string DisplayName => "Test Cheat Overlay";

        private Label _label;

        public VisualElement GetVisualElement()
        {
            return _label;
        }

        public CheatSubmenuAvailabilityInfo GetAvailability() => CheatSubmenuAvailabilityInfo.NotAvailable(
            $"This is a very long description of the reason of why this test overlay is not available at the moment. Current time: {UnityEngine.Time.time}");

        public async UniTask Load()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));

            _label = new Label();

            _label.text = "This is a test cheat overlay!";
        }

        public void OnShow()
        {
            Debug.Log("ON TEST OVERLAY SHOW!");
        }

        public void OnHide()
        {
            Debug.Log("ON TEST OVERLAY HIDE");
        }
    }
}
