﻿using ImGuiNET;
using Stride.Core.Mathematics;
using VL.Core.EditorAttributes;

namespace VL.ImGui.Widgets
{
    [GenerateNode(Name = "Input (Int4)", Category = "ImGui.Widgets", Tags = "number, updown")]
    [WidgetType(WidgetType.Input)]
    internal partial class InputInt4 : ChannelWidget<Int4>, IHasInputTextFlags
    {
        public int Step { private get; set; } = 1;

        public int StepFast { private get; set; } = 100;

        public ImGuiInputTextFlags Flags { get; set; }

        Int4 lastframeValue;

        internal override void UpdateCore(Context context)
        {
            var value = Update();
            if (ImGuiUtils.InputInt4(widgetLabel.Update(label.Value), ref value, Flags))
                SetValueIfChanged(lastframeValue, value, Flags);
            lastframeValue = value;
        }
    }
}
