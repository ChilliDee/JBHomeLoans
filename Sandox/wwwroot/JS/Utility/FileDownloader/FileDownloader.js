    window.downloadFileFromByteArray = function (fileName, byteArray) {
        const blob = new Blob([byteArray]);
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
    };

    window.downloadCompressedFile = async function (fileName, compressedByteArray) {
        const compressedBlob = new Blob([compressedByteArray]);
    const decompressedStream = compressedBlob.stream().pipeThrough(
    new DecompressionStream('gzip')
    );
    const decompressedBlob = await new Response(decompressedStream).blob();

    const url = URL.createObjectURL(decompressedBlob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
    };
