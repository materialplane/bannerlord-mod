param(
    [switch]$Force
)

$wikiDir = Join-Path $PSScriptRoot "wiki"
$manifestPath = Join-Path $wikiDir ".sync-manifest.json"
$php = "C:\laragon\bin\php\php-8.3.30-Win32-vs16-x64\php.exe"
$mwRun = Join-Path $PSScriptRoot "mediawiki\maintenance\run.php"

if (-not (Test-Path $wikiDir)) {
    Write-Error "Wiki directory not found at $wikiDir"
    exit 1
}

$manifest = @{}
if (Test-Path $manifestPath) {
    try {
        $raw = Get-Content $manifestPath -Raw | ConvertFrom-Json
        foreach ($prop in $raw.PSObject.Properties) {
            $manifest[$prop.Name] = $prop.Value
        }
    } catch {
        $manifest = @{}
    }
}

$wikiFiles = Get-ChildItem -Path $wikiDir -Filter "*.wiki"
$updatedCount = 0
$skippedCount = 0

foreach ($file in $wikiFiles) {
    $pageName = ($file.BaseName -replace "_", " ")
    $fileBytes = [System.IO.File]::ReadAllBytes($file.FullName)
    $hasher = [System.Security.Cryptography.SHA256]::Create()
    $currentHash = [System.BitConverter]::ToString($hasher.ComputeHash($fileBytes)) -replace "-"

    $previousHash = $manifest[$file.Name]

    if (-not $Force -and ($currentHash -eq $previousHash)) {
        Write-Host " [UNCHANGED] $($file.Name)" -ForegroundColor DarkGray
        $skippedCount++
        continue
    }

    $content = [System.IO.File]::ReadAllText($file.FullName, [System.Text.Encoding]::UTF8)
    
    $pinfo = New-Object System.Diagnostics.ProcessStartInfo
    $pinfo.FileName = $php
    $pinfo.Arguments = "`"$mwRun`" edit.php --user=`"Goflish`" --summary=`"Synced from $($file.Name)`" `"$pageName`""
    $pinfo.RedirectStandardInput = $true
    $pinfo.RedirectStandardOutput = $true
    $pinfo.RedirectStandardError = $true
    $pinfo.UseShellExecute = $false
    $pinfo.StandardOutputEncoding = [System.Text.Encoding]::UTF8

    $process = New-Object System.Diagnostics.Process
    $process.StartInfo = $pinfo
    [void]$process.Start()

    $process.StandardInput.Write($content)
    $process.StandardInput.Close()

    $out = $process.StandardOutput.ReadToEnd()
    $process.WaitForExit()

    if ($process.ExitCode -eq 0 -or $out -like "*done*") {
        Write-Host " [UPDATED]   $($file.Name) -> '$pageName'" -ForegroundColor Green
        $manifest[$file.Name] = $currentHash
        $updatedCount++
    } else {
        Write-Host " [FAILED]    $($file.Name): $out" -ForegroundColor Red
    }
}

$manifest | ConvertTo-Json -Depth 4 | Set-Content -Path $manifestPath -Encoding UTF8

if ($updatedCount -gt 0) {
    & $php $mwRun refreshLinks.php | Out-Null
    Write-Host "`nSync complete: $updatedCount page(s) updated, $skippedCount unchanged." -ForegroundColor Cyan
} else {
    Write-Host "`nAll $skippedCount page(s) are already up to date. No changes needed." -ForegroundColor Yellow
}
