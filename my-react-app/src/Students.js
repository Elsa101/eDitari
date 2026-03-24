function Students() {
  const getStudents = async () => {
    try {
      const response = await fetch("http://localhost:5102/api/Students");
      const data = await response.json();
      console.log(data);
      alert("Studentet u moren me sukses");
    } catch (error) {
      console.error(error);
      alert("Gabim gjate marrjes se studenteve");
    }
  };
 
  return (
<div>
<h2>Students Page</h2>
<button onClick={getStudents}>Get Students</button>
</div>
  );
}
 
export default Students;