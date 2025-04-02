$source_file = "\\prdpararc21\Archivage_Services\Archivage_DSI\Archivage_Commun_DSI\IGA\extraction_maf.zip"
$zip = [IO.Compression.ZipFile]::OpenRead($source_file)
$entries = $zip.Entries
$zip.Dispose()