import http from 'k6/http';
import { sleep } from 'k6';

export const options = {
  iterations: 10000,
  vus: 200
};

export default function () {
    let data = createData();
    let headers = {
      'Content-Type': 'application/json',
      "Host": "www.example.com"
    };

    http.post('http://localhost:8080/api/users', data, {headers: headers});
  
    sleep(0.3);
  }

  function createData() {
    return JSON.stringify({
      items: [
        { 
          username: "DogMaster"
        }
      ]
    });
  }