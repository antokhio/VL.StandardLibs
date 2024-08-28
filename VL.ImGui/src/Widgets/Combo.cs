﻿namespace VL.ImGui.Widgets
{
    [GenerateNode(Name = "Combo (String)", Category = "ImGui.Widgets", Tags = "dropdown, pulldown, enum")]
    internal partial class Combo : ChannelWidget<string>
    {
        public IEnumerable<string> Items { get; set; } = Enumerable.Empty<string>();

        public string? Format { private get; set; }

        public ImGuiNET.ImGuiComboFlags Flags { private get; set; }
        
        /// <summary>
        /// Returns true if content is visible.
        /// </summary>
        public bool ContentIsVisible { get; private set; } = false;

        internal override void UpdateCore(Context context)
        {
            var value = Update();

            if (Items != null && Items.Any())
            {
                ContentIsVisible = ImGuiNET.ImGui.BeginCombo(widgetLabel.Update(label.Value), value, Flags);

                if (ContentIsVisible)
                {
                    try
                    {
                        foreach (var item in Items)
                        {
                            bool is_selected = value == item;
                            if (ImGuiNET.ImGui.Selectable(item, is_selected))
                            {
                                Value = item;
                            }
                            if (is_selected)
                            {
                                ImGuiNET.ImGui.SetItemDefaultFocus();
                            }
                        }
                    }
                    finally
                    {
                        ImGuiNET.ImGui.EndCombo();
                    }
                }
            }
        }
    }
}
