
using BLIT64_Common;

namespace BLIT64_Editor
{
    public static class AppLayout
    {
        public static LayoutData Data { get; private set;}

        public static void Load()
        {
            Data = BonFileReader.Parse<LayoutData>("layout.bon");
        }

        public class LayoutData
        {
            public int EditorSizeMultiplier { get; set; }

            public int EditorCursorBorder { get; set; }

            public int EditorPanelBorder { get; set; }

            public int NavigatorSizeMultiplier { get; set; }

            public int ToolBoxWidth { get; set; }

            public int ToolBoxHeight { get; set; }

            public float ToolBoxIconsScale { get; set; }

            public int ToolBoxIconsSpacing { get; set; }

            public int ToolBoxPointerMargin { get; set; }

            public int ColorPickerWidth { get; set; }

            public int SelectorThumbSize { get; set; }

            public int NavigatorPanelBorder { get; set; }

            public int EditorPixmapSizeMultiplier { get; set; }

            public int SectionSelectorTabWidth { get; set; }

            public int SectionSelectorTabHeight { get; set; }

            /* OFFSETS */

            public int ToolBoxIconsShadowOffset { get; set; }

            public int NavigatorFrameLabelOffsetX { get;set;}

            public int NavigatorFrameLabelOffsetY { get; set; }

            public int EditorMousePosLabelOffsetX { get; set;}

            public int EditorMousePosLabelOffsetY { get; set; }
        }
    }
}
