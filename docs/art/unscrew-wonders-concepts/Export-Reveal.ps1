param([string]$Source)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$root = (Resolve-Path "$PSScriptRoot/../../..").Path
$art = Join-Path $root 'Assets/Resources/Art/Branding'
$sourceImage = [System.Drawing.Bitmap]::new($Source)
# Mechanical export only: fit generated alpha artwork into the adaptive safe circle.
$minX=$sourceImage.Width; $minY=$sourceImage.Height; $maxX=0; $maxY=0
for($y=0;$y -lt $sourceImage.Height;$y++) { for($x=0;$x -lt $sourceImage.Width;$x++) {
 if($sourceImage.GetPixel($x,$y).A -gt 8) { $minX=[Math]::Min($minX,$x); $minY=[Math]::Min($minY,$y); $maxX=[Math]::Max($maxX,$x); $maxY=[Math]::Max($maxY,$y) }
}}
$cx=($minX+$maxX)/2; $cy=($minY+$maxY)/2; $radius=0
for($y=$minY;$y -le $maxY;$y++) { for($x=$minX;$x -le $maxX;$x++) {
 if($sourceImage.GetPixel($x,$y).A -gt 8) { $radius=[Math]::Max($radius,[Math]::Sqrt([Math]::Pow($x-$cx,2)+[Math]::Pow($y-$cy,2))) }
}}
function New-Canvas { [System.Drawing.Bitmap]::new(1024,1024,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb) }
function Draw-Art($bitmap,$scale) {
 $g=[System.Drawing.Graphics]::FromImage($bitmap)
 $g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
 $g.DrawImage($sourceImage,[single](512-$cx*$scale),[single](512-$cy*$scale),[single]($sourceImage.Width*$scale),[single]($sourceImage.Height*$scale))
 $g.Dispose()
}
$foreground=New-Canvas
Draw-Art $foreground (298/$radius)
$foreground.Save((Join-Path $art 'Reveal_Foreground.png'))
$background=New-Canvas
$g=[System.Drawing.Graphics]::FromImage($background); $g.Clear([System.Drawing.ColorTranslator]::FromHtml('#075B70')); $g.Dispose()
$background.Save((Join-Path $art 'Reveal_Background.png'))
$legacy=[System.Drawing.Bitmap]::new($background)
Draw-Art $legacy (440/$radius)
$legacy.Save((Join-Path $art 'Reveal_Legacy.png'))
# Preview adaptive layers using a centered 72/108 viewport at launcher sizes.
$preview=[System.Drawing.Bitmap]::new(980,610)
$g=[System.Drawing.Graphics]::FromImage($preview); $g.Clear([System.Drawing.ColorTranslator]::FromHtml('#EEF2F4'))
$g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$font=[System.Drawing.Font]::new('Segoe UI',12); $title=[System.Drawing.Font]::new('Segoe UI',20)
$g.DrawString('The Reveal — launcher preview (simulation)',$title,[System.Drawing.Brushes]::Black,24,18)
$g.DrawString('Actual-size samples: 48, 64 and 96 pixels. Enlarged views at right.',$font,[System.Drawing.Brushes]::Black,24,60)
$row=0
foreach($shape in @('Circle','Rounded square')) {
 $y=125+$row*230; $g.DrawString($shape,$font,[System.Drawing.Brushes]::Black,24,$y)
 $x=190
 foreach($size in @(48,64,96,180)) {
  $state=$g.Save(); $path=[System.Drawing.Drawing2D.GraphicsPath]::new()
  if($shape -eq 'Circle') { $path.AddEllipse($x,$y,$size,$size) } else {
   $r=[int]($size*0.3); $path.AddArc($x,$y,$r,$r,180,90); $path.AddArc($x+$size-$r,$y,$r,$r,270,90); $path.AddArc($x+$size-$r,$y+$size-$r,$r,$r,0,90); $path.AddArc($x,$y+$size-$r,$r,$r,90,90); $path.CloseFigure()
  }
  $g.SetClip($path)
  $dest=[System.Drawing.Rectangle]::new($x,$y,$size,$size)
  foreach($layer in @($background,$foreground)) { $g.DrawImage($layer,$dest,[single](1024/6),[single](1024/6),[single](1024*2/3),[single](1024*2/3),[System.Drawing.GraphicsUnit]::Pixel) }
  $g.Restore($state); $path.Dispose()
  $g.DrawString("$size px",$font,[System.Drawing.Brushes]::Black,$x,$y+$size+8)
  $x+= $size+50
 }
 $row++
}
$g.DrawString('Provisional branding. Device launcher appearance may vary.',$font,[System.Drawing.Brushes]::Black,24,575)
$preview.Save((Join-Path $PSScriptRoot '05-phone-icon-preview.png'))
$g.Dispose(); $preview.Dispose(); $foreground.Dispose(); $background.Dispose(); $legacy.Dispose(); $sourceImage.Dispose(); $font.Dispose(); $title.Dispose()
Write-Output 'Exported adaptive foreground/background, legacy icon and launcher preview.'
