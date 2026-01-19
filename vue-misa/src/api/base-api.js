// src/api/baseApi.js
import axios from 'axios'

export class BaseApi {
  constructor(baseURL) {
    this.axiosInstance = axios.create({
      baseURL,
      headers: { 'Content-Type': 'application/json' },
    })
  }

  // GET request (nếu cần)
  async get(endpoint, params = {}) {
    try {
      const response = await this.axiosInstance.get(endpoint, { params })
      return response.data.data // trả về mảng data
    } catch (error) {
      console.error('API GET error:', error)
      return []
    }
  }

  // POST request
  async post(endpoint, payload = {}) {
    try {
      const response = await this.axiosInstance.post(endpoint, payload)
      return response.data.data // trả về luôn data
    } catch (error) {
      console.error('API POST error:', error)
      return []
    }
  }
}
