# Sincroniza la skill generic-sdd-agent del repositorio hacia el directorio de
# skills del usuario, para que los agentes puedan cargarla automaticamente.
#
# Uso:
#   powershell -File .agents/skills/generic-sdd-agent/scripts/sync-skill.ps1
#   powershell -File .agents/skills/generic-sdd-agent/scripts/sync-skill.ps1 -WhatIf
#
# La copia generada NO se edita a mano: la fuente de verdad es este repositorio.

[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [string] $Source,
    [string] $Destination
)

$ErrorActionPreference = 'Stop'

if (-not $Source) {
    $Source = Split-Path -Parent (Split-Path -Parent $PSCommandPath)
}

if (-not $Destination) {
    $Destination = Join-Path $env:USERPROFILE '.agents\skills\generic-sdd-agent'
}

$skillFile = Join-Path $Source 'SKILL.md'
if (-not (Test-Path $skillFile)) {
    throw "No se encontro SKILL.md en '$Source'. Verifica el parametro -Source."
}

Write-Host "Origen : $Source"
Write-Host "Destino: $Destination"

if ($PSCmdlet.ShouldProcess($Destination, 'Copiar skill')) {
    if (Test-Path $Destination) {
        Remove-Item -Recurse -Force $Destination
    }

    New-Item -ItemType Directory -Force -Path $Destination | Out-Null

    Copy-Item -Path (Join-Path $Source 'SKILL.md') -Destination $Destination -Force
    Copy-Item -Path (Join-Path $Source 'references') -Destination $Destination -Recurse -Force

    # Los scripts de mantenimiento no necesitan viajar al directorio de skills.
    Write-Host 'Skill sincronizada. Archivos copiados:'
    Get-ChildItem $Destination -Recurse -File |
        ForEach-Object { "  " + $_.FullName.Substring($Destination.Length + 1) }
}
