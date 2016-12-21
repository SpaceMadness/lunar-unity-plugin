//
//  CAboutWindow.cs
//
//  Lunar Plugin for Unity: a command line solution for your game.
//  https://github.com/SpaceMadness/lunar-unity-plugin
//
//  Copyright 2016 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

﻿using System;

using UnityEngine;
using UnityEditor;

namespace LunarEditor
{
    class CAboutWindow : CWindow
    {
        public CAboutWindow()
            : base("About Lunar")
        {
            Rect rect = this.position;
            rect.size = new Vector2(320, 240);
            this.position = rect;
        }

        protected override void CreateUI()
        {
            const float indent = 12;

            CView contentView = new CView(this.Width - 2 * indent, this.Height - 2 * indent);
            contentView.X = indent;
            contentView.Y = indent;
            AddSubview(contentView);

            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 20;
            CLabel titleLabel = new CLabel("Lunar Plugin for Unity", titleStyle);
            contentView.AddSubview(titleLabel);

            CLabel copyrightLabel = new CLabel("(C) 2015  Space Madness", EditorStyles.miniLabel);
            copyrightLabel.Height += 20;
            contentView.AddSubview(copyrightLabel);

            CView buttonsView = new CView();

            CButton twitterButton = new CButton("Follow @LunarPlugin", delegate(CButton button) {
                Application.OpenURL("https://twitter.com/intent/follow?screen_name=LunarPlugin&user_id=2939274198");
            });
            twitterButton.Width = contentView.Width;
            buttonsView.AddSubview(twitterButton);

            CButton facebookButton = new CButton("Facebook Page", delegate(CButton button) {
                Application.OpenURL("https://www.facebook.com/LunarPlugin");
            });
            facebookButton.Width = contentView.Width;
            buttonsView.AddSubview(facebookButton);

            CButton emailButton = new CButton("Send Email", delegate(CButton button) {
                Application.OpenURL("mailto:lunar.plugin@gmail.com?subject=Lunar%20plugin%20feedback&body=Hey%20Alex%2C%0A%0AI%27ve%20just%20checked%20the%20Lunar%20plugin.%20Nice%20job%2C%20man%21%20Still%2C%20there%27s%20some%20stuff%20I%20don%27t%20really%20like.%20For%20example%2C%20...%0A%0AFix%20it%2C%20you%20son%20of%20a%20bitch%2C%20or%20go%20to%20Hell%21%0A%0ASincerely%2C%0A%0AYour%20Lunar%20Plugin%20User");
            });
            emailButton.Width = contentView.Width;
            buttonsView.AddSubview(emailButton);

            buttonsView.ArrangeVert(5);
            buttonsView.ResizeToFitSubviews();
            buttonsView.Height += 20;

            contentView.AddSubview(buttonsView);

            contentView.ArrangeVert();

            CLabel thanksLabel = new CLabel("Thanks for using Lunar!");
            contentView.AddSubview(thanksLabel);
            thanksLabel.Align(0.5f, 1.0f);
        }

        internal static void ShowWindow()
        {
            EditorWindow.GetWindow<CAboutWindow>();
        }
    }
}

