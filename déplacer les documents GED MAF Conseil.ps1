# Définir les chemins source et destination
$sourcePath = "\\prdpararc21\Archivage_Services\Archivage_DSI\Archivage_Commun_DSI\IGA\extraction_maf.zip"
$destinationFolder = "\\prdparfic02\Applications\Production\GED\Extraction IGA"

# Vérifier si le fichier source existe
if (!(Test-Path $sourcePath)) {
    Write-Host "Le fichier source n'existe pas : $sourcePath" -ForegroundColor Red
    exit 1
}

# Créer le dossier de destination s'il n'existe pas
if (!(Test-Path $destinationFolder)) {
    New-Item -ItemType Directory -Path $destinationFolder -Force | Out-Null
}

$sourceDirectory = Split-Path -Path $sourcePath -Parent
$sourceFile = Split-Path -Path $sourcePath -Leaf
# Write-Host "source directory: $sourceDirectory, source file: $sourceFile"
# exit 0

# Utiliser Robocopy pour déplacer le fichier
Write-Host "Déplacement du fichier avec Robocopy..."
robocopy $sourceDirectory $destinationFolder $sourceFile /move /mt:8 /r:3 /w:5 /j /log:"$env:HOMEPATH\documents\Logs\Backup.log"

# Vérifier si le fichier a bien été déplacé
if (!(Test-Path $sourcePath) -and (Test-Path "$destinationFolder\$(Split-Path -Leaf $sourcePath)")) {
    Write-Host "Le fichier a été déplacé avec succès." -ForegroundColor Green
} else {
    Write-Host "Erreur lors du déplacement du fichier !" -ForegroundColor Red
}
