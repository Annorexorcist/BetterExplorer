﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace BetterExplorer
{
    public static class AppCommands
    {
        #region RoutedCommandDefinition
        public static RoutedCommand RoutedNewTab = new RoutedCommand();
        public static RoutedCommand RoutedEnterInBreadCrumbCombo = new RoutedCommand();
        public static RoutedCommand RoutedChangeTab = new RoutedCommand();
        public static RoutedCommand RoutedCloseTab = new RoutedCommand();
        public static RoutedCommand RoutedNavigateBack = new RoutedCommand();
        public static RoutedCommand RoutedNavigateFF = new RoutedCommand();
        public static RoutedCommand RoutedNavigateUp = new RoutedCommand();
        public static RoutedCommand RoutedGotoSearch = new RoutedCommand();
        #endregion
    }
}
