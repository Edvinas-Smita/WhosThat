param($inFile, $outFile)

[Byte[]]$bytes = Get-Content $inFile -Encoding Byte -ReadCount 0

$red = 0
$green = 0
$blue = 0
$avg = 0

[Byte[]]$gray = @(0) * (76800)#$bytes.Length
#for ($i = 0; $i -lt 63; $i++)	#copy metadata
#{
#	$gray[$i] = $bytes[$i]
#}

$gc = 0
for ($i = 54; $i -lt $bytes.Length; $i = $i + 3)	#copy pixel data
{
	$blue = $bytes[$i]
	$green = $bytes[$i + 1]
	$red = $bytes[$i + 2]
	
	$avg = (($blue + $green + $red) / 3)
	
	$gray[76799 - $gc++] = $avg
}

[io.file]::WriteAllBytes($outFile, $gray)
