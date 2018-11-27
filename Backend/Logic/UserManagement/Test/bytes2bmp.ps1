param($inFile, $outFile)

[Byte[]]$bytes = Get-Content $inFile -Encoding Byte -ReadCount 0

$red = 0
$green = 0
$blue = 0
$avg = 0

[Byte[]]$full = @(66,
77,
54,
132,
3,
0,
0,
0,
0,
0,
54,
0,
0,
0,
40,
0,
0,
0,
240,
0,
0,
0,
64,
1,
0,
0,
1,
0,
24,
0,
0,
0,
0,
0,
0,
0,
0,
0,
19,
11,
0,
0,
19,
11,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0)
$MetaByteCount = $full.Length
[Byte[]]$full += [Byte[]](@(0) * ($bytes.Length * 3))

for ($i = 0; $i -lt $bytes.Length; $i++)	#copy pixel data
{
	$full[$MetaByteCount + ($i * 3)] = $bytes[$bytes.Length - $i - 1]
	$full[$MetaByteCount + ($i * 3) + 1] = $bytes[$bytes.Length - $i - 1]
	$full[$MetaByteCount + ($i * 3) + 2] = $bytes[$bytes.Length - $i - 1]
}

[io.file]::WriteAllBytes($outFile, $full)
