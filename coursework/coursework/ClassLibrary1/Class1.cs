using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace music
{
    public struct Musicant
    {
        public string name { get; set; }
        public string surname { get; set; }
        public override string ToString()
        {
            return surname + " " + name;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Musicant))
            {
                return false;
            }

            Musicant otherMusicant = (Musicant)obj;
            return name == otherMusicant.name && surname == otherMusicant.surname;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public class Member<T> : IComparable<Member<T>>, IComparer<Member<T>>
    {
        public T member { get; set; }
        public int experience { get; set; }

        public Member(T member, int experience)
        {
            this.member = member;
            this.experience = experience;
        }

        public override string ToString()
        {
            return member.ToString() + " " + experience;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Member<T>))
            {
                return false;
            }

            Member<T> otherMember = (Member<T>)obj;
            return EqualityComparer<T>.Default.Equals(otherMember.member, member) && otherMember.experience == experience;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public int CompareTo(Member<T> other)
        {
            if (other == null)
            {
                return 1;
            }
            return string.Compare(this.member.ToString(), other.member.ToString());
        }
        public int Compare(Member<T> y, Member<T> x)
        {
            if (x == null && y == null)
                return 0;
            if (x == null)
                return -1;
            if (y == null)
                return 1;
            if (x.experience > y.experience)
                return 1;
            if (x.experience < y.experience)
                return -1;
            return 0;
        }
    }

    public class CreativeTeam
    {
        public string name;
        public Member<Musicant>[] members;
        public CreativeTeam(string name, Member<Musicant>[] members)
        {
            this.name = name;
            this.members = members;
        }
        public void Add(Member<Musicant> musicant)
        {
            Member<Musicant>[] newMembers = members;
            Array.Resize(ref newMembers, members.Length + 1);
            members = newMembers;
            members[members.Length - 1] = musicant;
        }
        public void Remove(Musicant musicant)
        {
            int i, k;
            Member<Musicant>[] newMembers = new Member<Musicant>[members.Length];
            for (i = 0, k = 0; i < members.Length; i++, k++)
            {
                if (members[i].Equals(musicant))
                {
                    k--;
                }
                else
                {
                    newMembers[k] = members[i];
                }
            }
            if (i > k)
            {
                Array.Resize(ref newMembers, members.Length - 1);
            }
            members = newMembers;
        }
        public static CreativeTeam operator +(CreativeTeam team, Member<Musicant> musicant)
        {
            CreativeTeam newTeam = new CreativeTeam(team.name, team.members);
            newTeam.Add(musicant);
            return newTeam;
        }
        public static CreativeTeam operator -(CreativeTeam team, Musicant musicant)
        {
            CreativeTeam newTeam = new CreativeTeam(team.name, team.members);
            newTeam.Remove(musicant);
            return newTeam;
        }
        public override string ToString()
        {
            string s = "Назва: " + name + "\nУчасники: ";
            for (int i = 0; i < members.Length; i++)
            {
                s += "\n" + members[i].ToString();
            }
            return s;
        }
        public void SortByAlphabet()
        {
            Array.Sort(members);
        }
        public void SortByExperience()
        {
            Array.Sort(members, members[0]);
        }
    }
    public class MusicBand : CreativeTeam
    {
        public string genre;
        public MusicBand(string name, Member<Musicant>[] members, string genre) : base(name, members)
        {
            this.genre = genre;
        }
        public MusicBand(): base("", new Member<Musicant>[0])
        {
            genre = "";
        }
        public override string ToString()
        {
            return base.ToString() + "\nЖанр: " + genre;
        }
        public static MusicBand operator +(MusicBand team, Member<Musicant> musicant)
        {
            MusicBand newTeam = new MusicBand(team.name, team.members, team.genre);
            newTeam.Add(musicant);
            return newTeam;
        }
        public static MusicBand operator -(MusicBand team, Musicant musicant)
        {
            MusicBand newTeam = new MusicBand(team.name, team.members, team.genre);
            newTeam.Remove(musicant);
            return newTeam;
        }
        public void FromFile(string filePath)
        {
            try
            {
                using (StreamReader fileStream = new StreamReader(filePath))
                {
                    List<Member<Musicant>> mbrs = new List<Member<Musicant>>();
                    string nm = "", gnr = "";
                    string line;
                    while ((line = fileStream.ReadLine()) != null)
                    {
                        if (nm == "")
                        {
                            nm = line.Split(':')[1].Trim();
                        }
                        else if (line.Trim() != "Учасники:" && !line.StartsWith("Жанр:"))
                        {
                            string[] memberInfo = line.Split(' ');
                            Musicant musicant = new Musicant { surname = memberInfo[0], name = memberInfo[1] };
                            Member<Musicant> member = new Member<Musicant>(musicant, int.Parse(memberInfo[2]));
                            mbrs.Add(member);
                        }
                        else if (line.StartsWith("Жанр:"))
                        {
                            gnr = line.Substring(line.IndexOf(':') + 1).Trim();
                        }
                    }

                    this.members = mbrs.ToArray();
                    this.name = nm;
                    this.genre = gnr;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении из файла: {ex.Message}");
            }
        }
        public void SaveToFile(string filePath)
        {
            try
            {
                using (StreamWriter fileWriter = new StreamWriter(filePath))
                {
                    fileWriter.Write(this.ToString());
                }
                Console.WriteLine("Данi успiшно збережено у файлi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка збереження в файл: " + ex.Message);
            }
        }
    }
}
