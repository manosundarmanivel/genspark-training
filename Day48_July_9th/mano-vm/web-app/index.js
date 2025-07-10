const express = require('express');
const axios = require('axios');
const app = express();
const port = 3000;

app.get('/', async (req, res) => {
  try {
    const response = await axios.get('https://jsonplaceholder.typicode.com/posts?_limit=5');
    const posts = response.data;

    let html = `<h1>Top 5 Posts from JSONPlaceholder</h1><ul>`;
    posts.forEach(post => {
      html += `<li><strong>${post.title}</strong><br>${post.body}</li><br>`;
    });
    html += `</ul>`;
    res.send(html);
  } catch (error) {
    res.status(500).send('Failed to fetch posts');
  }
});

app.listen(port, () => {
  console.log(`App listening at http://localhost:${port}`);
});
