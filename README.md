# Steps to Run
- In one terminal, enter `docker run -p 6333:6333 qdrant/qdrant` to run the vector DB.
- In another terminal, enter `dotnet user-secrets set "OpenAI:ApiKey" "your-api-key-here"` to set your openAI key (used for embeddings).
- Enter `dotnet run` to launch the background services and the backend.
- In another terminal, run `cd oce-agent-ui` & enter `npm run dev` to launch the frontend.
