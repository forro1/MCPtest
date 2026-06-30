Add-Type -AssemblyName System.Drawing

$outDir = Join-Path (Get-Location) "Assets\Art\Cards"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

$fontTitle = New-Object System.Drawing.Font("Microsoft YaHei", 34, [System.Drawing.FontStyle]::Bold)
$fontMeta = New-Object System.Drawing.Font("Microsoft YaHei", 22, [System.Drawing.FontStyle]::Bold)
$fontBody = New-Object System.Drawing.Font("Microsoft YaHei", 20, [System.Drawing.FontStyle]::Regular)
$fontCost = New-Object System.Drawing.Font("Microsoft YaHei", 32, [System.Drawing.FontStyle]::Bold)

function ColorFromHex($hex) {
    return [System.Drawing.ColorTranslator]::FromHtml($hex)
}

function New-Brush($hex) {
    return New-Object System.Drawing.SolidBrush((ColorFromHex $hex))
}

function New-Pen($hex, $width) {
    return New-Object System.Drawing.Pen((ColorFromHex $hex), $width)
}

function Pt($x, $y) {
    return [System.Drawing.PointF]::new([float]$x, [float]$y)
}

function Draw-CenteredText($g, $text, $font, $brush, $rect) {
    $fmt = New-Object System.Drawing.StringFormat
    $fmt.Alignment = [System.Drawing.StringAlignment]::Center
    $fmt.LineAlignment = [System.Drawing.StringAlignment]::Center
    $g.DrawString($text, $font, $brush, $rect, $fmt)
    $fmt.Dispose()
}

function Draw-WrappedText($g, $text, $font, $brush, $rect) {
    $fmt = New-Object System.Drawing.StringFormat
    $fmt.Alignment = [System.Drawing.StringAlignment]::Near
    $fmt.LineAlignment = [System.Drawing.StringAlignment]::Near
    $fmt.Trimming = [System.Drawing.StringTrimming]::EllipsisWord
    $g.DrawString($text, $font, $brush, $rect, $fmt)
    $fmt.Dispose()
}

function Draw-RoundedRect($g, $pen, $brush, $x, $y, $w, $h, $r) {
    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $d = $r * 2
    $path.AddArc($x, $y, $d, $d, 180, 90)
    $path.AddArc($x + $w - $d, $y, $d, $d, 270, 90)
    $path.AddArc($x + $w - $d, $y + $h - $d, $d, $d, 0, 90)
    $path.AddArc($x, $y + $h - $d, $d, $d, 90, 90)
    $path.CloseFigure()
    if ($brush -ne $null) { $g.FillPath($brush, $path) }
    if ($pen -ne $null) { $g.DrawPath($pen, $path) }
    $path.Dispose()
}

function Draw-Diamond($g, $brush, $pen, $cx, $cy, $r) {
    [System.Drawing.PointF[]]$pts = @(
        (Pt $cx ($cy - $r)),
        (Pt ($cx + $r) $cy),
        (Pt $cx ($cy + $r)),
        (Pt ($cx - $r) $cy)
    )
    $g.FillPolygon($brush, $pts)
    $g.DrawPolygon($pen, $pts)
}

function Draw-Icon($g, $kind, $accent, $darkPen) {
    $glow = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(70, $accent))
    $main = New-Object System.Drawing.SolidBrush($accent)
    $light = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(230, 255, 245, 210))
    $penLight = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(220, 255, 235, 180), 8)
    $penAccent = New-Object System.Drawing.Pen($accent, 12)

    $g.FillEllipse($glow, 96, 148, 320, 220)

    switch ($kind) {
        "Attack" {
            $g.DrawLine($penLight, 160, 338, 346, 162)
            $g.DrawLine($penAccent, 182, 350, 366, 176)
            $g.DrawLine($darkPen, 150, 356, 370, 150)
            [System.Drawing.PointF[]]$tip = @(
                (Pt 350 146),
                (Pt 392 124),
                (Pt 372 170)
            )
            $g.FillPolygon($main, $tip)
            $g.DrawLine($darkPen, 170, 236, 250, 318)
        }
        "Defense" {
            [System.Drawing.PointF[]]$pts = @(
                (Pt 256 142),
                (Pt 352 182),
                (Pt 330 326),
                (Pt 256 374),
                (Pt 182 326),
                (Pt 160 182)
            )
            $g.FillPolygon($main, $pts)
            $g.DrawPolygon($darkPen, $pts)
            $g.DrawLine($penLight, 256, 158, 256, 350)
            $g.DrawRectangle($darkPen, 146, 308, 58, 36)
            $g.DrawRectangle($darkPen, 318, 308, 52, 36)
        }
        "Fire" {
            [System.Drawing.PointF[]]$flame = @(
                (Pt 260 370),
                (Pt 190 296),
                (Pt 232 244),
                (Pt 222 160),
                (Pt 290 222),
                (Pt 308 150),
                (Pt 350 242),
                (Pt 326 318)
            )
            $g.FillClosedCurve($main, $flame)
            $g.DrawClosedCurve($darkPen, $flame)
            $g.FillEllipse($light, 236, 270, 66, 78)
        }
        "Heal" {
            $g.FillEllipse($main, 216, 190, 84, 142)
            $g.DrawEllipse($darkPen, 216, 190, 84, 142)
            $g.DrawLine($darkPen, 230, 190, 286, 190)
            $g.DrawLine($penLight, 256, 212, 256, 306)
            $g.DrawLine($penLight, 222, 260, 290, 260)
            $g.FillEllipse($glow, 168, 250, 78, 48)
            $g.FillEllipse($glow, 276, 250, 78, 48)
        }
        "Heavy" {
            $g.DrawLine((New-Object System.Drawing.Pen((ColorFromHex "#2a1c35"), 28)), 190, 176, 336, 322)
            $g.FillRectangle($main, 280, 128, 108, 78)
            $g.DrawRectangle($darkPen, 280, 128, 108, 78)
            $g.DrawLine($penAccent, 170, 360, 256, 300)
            $g.DrawLine($penAccent, 256, 300, 346, 364)
            $g.DrawLine($penAccent, 256, 300, 256, 388)
        }
    }

    $glow.Dispose()
    $main.Dispose()
    $light.Dispose()
    $penLight.Dispose()
    $penAccent.Dispose()
}

$cards = @(
    @{ File="attack_slash"; Name="斩击"; Type="攻击"; Cost="1"; Effect="造成6点伤害。"; Kind="Attack"; Accent="#d94a3a" },
    @{ File="attack_double_slash"; Name="连斩"; Type="攻击"; Cost="1"; Effect="造成3点伤害2次。"; Kind="Attack"; Accent="#d94a3a" },
    @{ File="attack_pierce"; Name="破甲击"; Type="攻击"; Cost="2"; Effect="造成9点伤害，并削减3点格挡。"; Kind="Attack"; Accent="#d94a3a" },
    @{ File="defense_guard"; Name="格挡"; Type="防御"; Cost="1"; Effect="获得6点格挡。"; Kind="Defense"; Accent="#3f85d7" },
    @{ File="defense_stone_wall"; Name="石墙"; Type="防御"; Cost="2"; Effect="获得12点格挡。"; Kind="Defense"; Accent="#3f85d7" },
    @{ File="defense_counter"; Name="反制"; Type="防御"; Cost="1"; Effect="获得4点格挡。下次受击后反击3点伤害。"; Kind="Defense"; Accent="#3f85d7" },
    @{ File="fire_spark"; Name="火花"; Type="法术"; Cost="0"; Effect="造成3点伤害。"; Kind="Fire"; Accent="#ee9a22" },
    @{ File="fire_fireball"; Name="火球"; Type="法术"; Cost="2"; Effect="造成10点伤害。"; Kind="Fire"; Accent="#ee9a22" },
    @{ File="fire_ember"; Name="余烬"; Type="法术"; Cost="1"; Effect="造成5点伤害。若击败敌人，抽1张牌。"; Kind="Fire"; Accent="#ee9a22" },
    @{ File="heal_mend"; Name="治疗"; Type="治疗"; Cost="1"; Effect="恢复5点生命。"; Kind="Heal"; Accent="#4fb85d" },
    @{ File="heal_regrowth"; Name="复苏"; Type="治疗"; Cost="2"; Effect="恢复8点生命，并获得2点格挡。"; Kind="Heal"; Accent="#4fb85d" },
    @{ File="heal_vitality"; Name="活力"; Type="治疗"; Cost="0"; Effect="恢复2点生命。抽1张牌。"; Kind="Heal"; Accent="#4fb85d" },
    @{ File="heavy_bash"; Name="重击"; Type="重击"; Cost="2"; Effect="造成12点伤害。"; Kind="Heavy"; Accent="#8b55c7" },
    @{ File="heavy_crush"; Name="粉碎"; Type="重击"; Cost="3"; Effect="造成16点伤害，并移除敌人全部格挡。"; Kind="Heavy"; Accent="#8b55c7" },
    @{ File="heavy_shockwave"; Name="震荡"; Type="重击"; Cost="2"; Effect="造成8点伤害。敌人下回合意图伤害-3。"; Kind="Heavy"; Accent="#8b55c7" }
)

foreach ($card in $cards) {
    $bmp = New-Object System.Drawing.Bitmap(512, 768)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit

    $accent = ColorFromHex $card.Accent
    $bg = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
        (New-Object System.Drawing.Rectangle(0, 0, 512, 768)),
        (ColorFromHex "#f1dfb9"),
        (ColorFromHex "#a47b48"),
        90
    )
    $g.FillRectangle($bg, 0, 0, 512, 768)

    $outerBrush = New-Brush "#2b2118"
    $innerBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(235, 64, 43, 30))
    $parchment = New-Brush "#dbc194"
    $darkText = New-Brush "#2b2118"
    $whiteText = New-Brush "#fff4d6"
    $goldPen = New-Pen "#d7a94d" 7
    $darkPen = New-Pen "#261b14" 5
    $accentBrush = New-Object System.Drawing.SolidBrush($accent)
    $accentPen = New-Object System.Drawing.Pen($accent, 6)

    Draw-RoundedRect $g $goldPen $outerBrush 28 28 456 712 28
    Draw-RoundedRect $g $darkPen $innerBrush 48 52 416 668 20
    Draw-RoundedRect $g $accentPen $null 58 62 396 648 16
    Draw-RoundedRect $g $darkPen (New-Brush "#493523") 76 118 360 292 16
    Draw-RoundedRect $g $darkPen $parchment 76 430 360 242 14

    Draw-Icon $g $card.Kind $accent $darkPen

    $g.FillEllipse($accentBrush, 54, 54, 74, 74)
    $g.DrawEllipse($goldPen, 54, 54, 74, 74)
    Draw-CenteredText $g $card.Cost $fontCost $whiteText (New-Object System.Drawing.RectangleF(54, 54, 74, 74))

    Draw-Diamond $g $accentBrush $goldPen 256 88 23
    Draw-Diamond $g $accentBrush $goldPen 256 704 22

    Draw-CenteredText $g $card.Name $fontTitle $darkText (New-Object System.Drawing.RectangleF(96, 444, 320, 48))
    Draw-CenteredText $g ("类型：" + $card.Type + "    花费：" + $card.Cost) $fontMeta $darkText (New-Object System.Drawing.RectangleF(90, 502, 332, 38))
    Draw-WrappedText $g ("效果：" + $card.Effect) $fontBody $darkText (New-Object System.Drawing.RectangleF(104, 556, 304, 92))

    for ($i = 0; $i -lt 18; $i++) {
        $x = Get-Random -Minimum 42 -Maximum 470
        $y = Get-Random -Minimum 42 -Maximum 720
        $g.DrawLine((New-Pen "#6b5135" 1), $x, $y, ($x + (Get-Random -Minimum -12 -Maximum 13)), ($y + (Get-Random -Minimum -12 -Maximum 13)))
    }

    $path = Join-Path $outDir ($card.File + ".png")
    $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)

    $g.Dispose()
    $bmp.Dispose()
    $bg.Dispose()
    $outerBrush.Dispose()
    $innerBrush.Dispose()
    $parchment.Dispose()
    $darkText.Dispose()
    $whiteText.Dispose()
    $goldPen.Dispose()
    $darkPen.Dispose()
    $accentBrush.Dispose()
    $accentPen.Dispose()
}

Write-Output "Generated $($cards.Count) card assets in $outDir"
