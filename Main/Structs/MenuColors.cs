﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Main.Structs
{
    struct MenuColors
    {
        public MenuColors()
        {

        }

        public SolidColorBrush DisableColor => color_disable;
        public SolidColorBrush ActiveColor => color_active;

        SolidColorBrush color_disable = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
        SolidColorBrush color_active = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFACD1FF"));
    }
}