///<reference lib="webworker" />

addEventListener('message', async ({ data }) => {
  const {
    file,
    courseId,
    topic,
    description,
    chunkSize,
    uploadUrl,
    token
  } = data;

  const totalChunks = Math.ceil(file.size / chunkSize);

  for (let i = 0; i < totalChunks; i++) {
    const start = i * chunkSize;
    const end = Math.min(file.size, start + chunkSize);
    const chunk = file.slice(start, end);

    const formData = new FormData();
    formData.append('chunk', chunk);
    formData.append('chunkIndex', i.toString());
    formData.append('totalChunks', totalChunks.toString());
    formData.append('fileName', file.name);
    formData.append('courseId', courseId);
    formData.append('topic', topic);
    formData.append('description', description);

    try {
      const response = await fetch(uploadUrl, {
        method: 'POST',
        body: formData,
        headers: {
          Authorization: `Bearer ${token}` 
        }
      });

      if (!response.ok) {
        throw new Error(`Chunk ${i} upload failed with status ${response.status}`);
      }

      postMessage({ progress: Math.round(((i + 1) / totalChunks) * 100) });
    } catch (err) {
      let errorMessage = 'Unknown error';
      if (err instanceof Error) {
        errorMessage = err.message;
      }
      postMessage({ error: `Failed at chunk ${i}: ${errorMessage}` });
      return;
    }
  }

  postMessage({ done: true });
});
