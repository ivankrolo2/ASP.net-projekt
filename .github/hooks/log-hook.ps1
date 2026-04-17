param(
    [Parameter(Mandatory = $true)]
    [string]$EventName
)

$ErrorActionPreference = 'Stop'

try {
    $payload = [Console]::In.ReadToEnd()

    if ([string]::IsNullOrWhiteSpace($payload)) {
        $payload = '<empty-payload>'
    }

    $repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
    $logDir = Join-Path $repoRoot '.github\hooks'
    if (-not (Test-Path -Path $logDir)) {
        New-Item -Path $logDir -ItemType Directory -Force | Out-Null
    }

    $logPath = Join-Path $logDir 'agent_log.txt'
    $timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss.fff'
    $entry = "[$timestamp] [$EventName]`r`n$payload`r`n------------------------------`r`n"

    Add-Content -Path $logPath -Value $entry -Encoding UTF8
    exit 0
}
catch {
    # Keep hook non-blocking but leave a trace for diagnosis.
    try {
        $repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
        $fallbackLog = Join-Path $repoRoot '.github\hooks\agent_log.txt'
        $timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss.fff'
        Add-Content -Path $fallbackLog -Value "[$timestamp] [HookError:$EventName] $($_.Exception.Message)" -Encoding UTF8
    }
    catch {
    }

    exit 0
}
