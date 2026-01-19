// src/api/shiftApi.js
import axios from 'axios'

const BASE_URL = 'https://localhost:7136/api'

/**
 * getShifts thực hiện gọi đến api /shifts/filter
 * medthod post
 * đầu vào Payload
 * Trả về mảng data
 * Create by: NHDuy (13/1/2026)
 */
export async function getShifts(payload = {}) {
  try {
    const response = await axios.post(`${BASE_URL}/shifts/filter`, payload, {
      headers: { 'Content-Type': 'application/json' },
    })
    // Trả về luôn data và total
    return response.data
  } catch (error) {
    return { total: 0, data: [] }
  }
}
/**
 * Gán dữ liệu vào addShift
 * đầu vào mảng payload gồm các giá trị cố định shiftcode,shiftname....
 * trả về dữ liệu của be
 * Create by: NHDuy (13/1/2026)
 */
export async function addShift(payload) {
  try {
    const response = await axios.post(`${BASE_URL}/shifts`, payload, {
      headers: { 'Content-Type': 'application/json' },
    })
    return response
  } catch (error) {
    throw error
  }
}
/**
 * Gán dữ liệu vào updateShift
 * đầu vào mảng payload và id của obj cần sửa
 * dữ liệu trả về response của be
 * Create by: NHDuy (13/1/2026)
 */
export async function updateShift(id, payload) {
  try {
    console.log(payload)
    const response = await axios.put(`${BASE_URL}/shifts/${id}`, payload, {
      headers: { 'Content-Type': 'application/json' },
    })
    return response
  } catch (error) {
    throw error
  }
}
/**
 * Gán dữ liệu vào updateInactiveShifts
 * đầu vào mảng payload gồm arr chứa các ID cần set
 * dữ liệu trả về response của be
 * Create by: NHDuy (13/1/2026)
 */
export async function updateInactiveShifts(payload) {
  try {
    const response = await axios.put(`${BASE_URL}/shifts/inactive/bulk`, payload, {
      headers: { 'Content-Type': 'application/json' },
    })
    return response
  } catch (error) {
    throw error
  }
}
/**
 * Gán dữ liệu vào deleteManyShifts
 * đầu vào mảng ids chứa các id cần delete
 * dữ liệu trả về response của be
 * Create by: NHDuy (13/1/2026)
 */
export async function deleteManyShifts(ids) {
  try {
    const response = await axios.delete(`${BASE_URL}/shifts/bulk`, {
      data: ids,
      headers: { 'Content-Type': 'application/json' },
    })
    return response
  } catch (error) {
    throw error
  }
}
