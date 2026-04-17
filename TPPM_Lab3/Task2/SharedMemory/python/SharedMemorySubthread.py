import mmap
import time
import win32event
import win32con

def main():
    
    shm = mmap.mmap(-1, 1024, tagname="MTPP_Mem")
        
    data_ready_event = win32event.OpenEvent(2031619, False, "MTPP_Mem_DataReady")
    data_processed_event = win32event.OpenEvent(2031619, False, "MTPP_Mem_DataProcessed")

    try:
        with open('shared_mem_log_python.txt', 'a') as f:
            while True:
                win32event.WaitForSingleObject(data_ready_event, win32event.INFINITE)
                
                shm.seek(0)
                raw_bytes = shm.read(4)
                
                number = int.from_bytes(raw_bytes, byteorder='little', signed=True)
                

                f.write(f"Sub-thread int recieved back:{number};\n")
                f.flush() 
                
                result = number * -1
                result_bytes = result.to_bytes(4, byteorder='little', signed=True)
                
                shm.seek(0)
                shm.write(result_bytes)

                win32event.SetEvent(data_processed_event)
            
    finally:
        shm.close()

if __name__ == "__main__":
    main()