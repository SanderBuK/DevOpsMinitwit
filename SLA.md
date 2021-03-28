# SLA for Minitwit

### Definitions
- **Uptime**: The service is running, and available through the IP.
- **Blazor**: The frontend part of our application.
- **API**: The backend part of out application.
- **Public timeline**: The "front page" of the website, located at `/public`.
- **Private timeline**: The "profile page" of the website, located at `/`.
- **User timeline**: A specific users timeline, located at `/{username}`.

### Uptime
- **Blazor**
  ~95% Uptime
- **API**
  ~95% Uptime
  
### Response time
- **Public timeline**: ~1 sec
- **Private timeline**: ~20 sec
- **User timeline**: ~1 sec
- **Posting data**: ~1 sec

### Recovery time
- **Average time from error is noticed, and notified**: ~10 min

### Failure frequency
- Overview at http://138.68.93.2/status.html under group i
